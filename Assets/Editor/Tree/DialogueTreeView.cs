using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;
using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Edit.Tree.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Edit.Search;
using Chocolate4.Edit.Graph.Utilities;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Tree
{
    [Serializable]
    public class DialogueTreeView : IRebuildable<TreeSaveData>, ISearchable
    {
        private TreeSaveData cachedTreeItems;
        private int cachedSelectedId;
        private bool shouldCacheTree;

        internal TreeView TreeView { get; private set; }

        internal event Action<string> OnSituationSelected;
        internal event Action<string> OnTreeItemRemoved;
        internal event Action<string> OnTreeItemAdded;
        internal event Action<string> OnTreeItemRenamed;

        public IEnumerable<DialogueTreeItem> DialogueTreeItems
        {
            get
            {
                List<int> itemIds = TreeView.viewController.GetAllItemIds().ToList();

                foreach (int id in itemIds)
                {
                    yield return TreeView.GetItemDataForId<DialogueTreeItem>(id);
                }
            }
        }

        internal void Initialize(TreeSaveData treeSaveData)
        {
            Rebuild(treeSaveData);
        }

        public void Rebuild(TreeSaveData treeSaveData)
        {
            cachedTreeItems = treeSaveData;
            CreateTreeView();

            RebuildTree(treeSaveData);
        }

        private void RebuildTree(TreeSaveData treeSaveData)
        {
            shouldCacheTree = true;
            var items = new List<TreeViewItemData<DialogueTreeItem>>();

            TreeItemSaveData[] rootDatas =
                treeSaveData.treeItemData.Where(itemSaveData => itemSaveData.depth == 0).ToArray();

            int rootElementCount = rootDatas.Length;
            for (int i = 0; i < rootElementCount; i++)
            {
                TreeItemSaveData rootItemSaveData = rootDatas[i];
                items.Add(
                    new TreeViewItemData<DialogueTreeItem>(i, rootItemSaveData.rootItem,
                        TreeUtilities.GetChildren(treeSaveData, rootItemSaveData, rootElementCount + i)
                    )
                );
            }

            TreeView.SetRootItems(items);

            TreeView.SetSelection(treeSaveData.selectedIndex);
            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);
        }

        public TreeSaveData Save()
        {
            return StructureSaver.SaveTree(TreeView);
        }

        public void Search(string value)
        {
            if (shouldCacheTree)
            {
                cachedTreeItems = Save();
                shouldCacheTree = false;
            }

            if (string.IsNullOrEmpty(value))
            {
                cachedTreeItems.selectedIndex = TreeView.viewController.GetIndexForId(cachedSelectedId);
                RebuildTree(cachedTreeItems);
                return;
            }

            TreeUtilities.FilterTreeViewBy(value, TreeView, OnSelectionChanged);
        }

        internal void RemoveTreeItem(DialogueTreeItem item, int index)
        {
            if (TreeView.GetTreeCount() <= 1)
            {
                return;
            }

            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);

            int id = TreeView.GetIdForIndex(index);
            if (!TreeView.TryRemoveItem(id))
            {
                return;
            }

            OnTreeItemRemoved?.Invoke(item.id);
        }

        internal DialogueTreeItem AddTreeItem(string defaultName, int index = -1, string idOverride = "")
        {
            int groupID = TreeView.GetIdForIndex(index);

            string[] existingNames = DialogueTreeItems.Select(item => item.displayName).ToArray();
            string name = ObjectNames.GetUniqueName(existingNames, defaultName);

            DialogueTreeItem treeItem = new DialogueTreeItem(name);
            AddItemToGroup(treeItem, groupID);

            if (!idOverride.Equals(string.Empty))
            {
                treeItem.id = idOverride;
            }

            OnTreeItemAdded?.Invoke(treeItem.id);
            return treeItem;
        }

        internal void GraphView_OnSituationCached(string situationGuid)
        {
            int count = TreeView.GetTreeCount();
            for (int i = 0; i < count; i++)
            {
                DialogueTreeItem item = TreeView.GetItemDataForIndex<DialogueTreeItem>(i);

                if (item.id.Equals(situationGuid))
                {
                    return;
                }
            }

            AddTreeItem(
                TreeViewConstants.DefaultSituationName, idOverride:situationGuid
            );
        }

        private void AddItemToGroup(DialogueTreeItem treeItem, int groupID)
        {
            int itemId = GUID.Generate().GetHashCode();
            TreeView.AddItem(
                new TreeViewItemData<DialogueTreeItem>(itemId, treeItem), groupID
            );

            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);
            TreeView.SetSelection(TreeView.viewController.GetIndexForId(itemId));
        }

        private void BindTreeViewItem(VisualElement element, int index)
        {
            DialogueTreeItem item = TreeView.GetItemDataForIndex<DialogueTreeItem>(index);

            Label renamableLabel = element.ElementAt(element.childCount - 1) as Label;
            renamableLabel.text = item.displayName;


            element.AddContextualMenu("Add Situation", _ => 
                AddTreeItem(TreeViewConstants.DefaultSituationName, index)
            );

            element.AddContextualMenu("Rename", _ => {

                string[] existingNames = DialogueTreeItems.Select(item => item.displayName).ToArray();
                VisualElementBuilder.Rename(renamableLabel, item.displayName, existingNames, finishedText => {
                    item.displayName = finishedText;
                    OnTreeItemRenamed?.Invoke(item.id);
                });
            }
            );

            element.AddContextualMenu("Remove", _ => RemoveTreeItem(item, index));
        }

        private VisualElement MakeTreeViewItem()
        {
            VisualElement box = new VisualElement().WithHorizontalGrow();

            Image image = new Image() { image = TreeUtilities.GetSituationIcon() };
            image.WithMaxWidth(UIStyles.ListViewItemHeight);
            image.style.justifyContent = Justify.FlexStart;

            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleLeft;

            box.Add(image);
            box.Add(label);
            return box;
        }

        private void CreateTreeView()
        {
            TreeView = new TreeView() { reorderable = true };

            TreeView.viewDataKey = "dialogue-tree";
            TreeView.fixedItemHeight = UIStyles.ListViewItemHeight;

            TreeView.makeItem = MakeTreeViewItem;
            TreeView.bindItem = BindTreeViewItem;
            TreeView.selectedIndicesChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(IEnumerable<int> selectedIndices)
        {
            if (!selectedIndices.Any())
                return;

            int index = selectedIndices.First();

            DialogueTreeItem selectedSituation = 
                TreeView.GetItemDataForIndex<DialogueTreeItem>(index);

            if (selectedSituation == null)
            {
                return;
            }

            cachedSelectedId = TreeView.GetIdForIndex(index);
            OnSituationSelected?.Invoke(selectedSituation.id);
        }
    }
}
