using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Dialogue.Runtime.Utilities;
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
        public TreeView TreeView { get; private set; }

        public event Action<string> OnSituationSelected;
        public event Action<string> OnTreeItemRemoved;
        public event Action<string> OnTreeItemAdded;
        public event Action<string> OnTreeItemRenamed;

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

        public void Initialize(TreeSaveData treeSaveData)
        {
            Rebuild(treeSaveData);
        }

        public void Rebuild(TreeSaveData treeSaveData)
        {
            TreeView = new TreeView() { reorderable = true };

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

            CreateTreeView();

            TreeView.SetSelection(treeSaveData.selectedIndex);
        }

        public TreeSaveData Save()
        {
            return StructureSaver.SaveTree(TreeView);
        }

        public void RemoveTreeItem(DialogueTreeItem item, int index)
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

        public DialogueTreeItem AddTreeItem(string defaultName, int index = -1, string idOverride = "")
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

        public void Search(string value)
        {
        }

        public void GraphView_OnSituationCached(string situationGuid)
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

            string[] existingNames = DialogueTreeItems.Select(item => item.displayName).ToArray();

            element.AddContextualMenu("Add Situation", _ => 
                AddTreeItem(TreeViewConstants.DefaultSituationName, index)
            );

            element.AddContextualMenu("Rename", _ =>
                VisualElementBuilder.Rename(renamableLabel, existingNames, finishedText => {
                    item.displayName = finishedText;
                    OnTreeItemRenamed?.Invoke(item.id);
                })
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
            TreeView.viewDataKey = "dialogue-tree";
            TreeView.fixedItemHeight = UIStyles.ListViewItemHeight;

            TreeView.makeItem = MakeTreeViewItem;
            TreeView.bindItem = BindTreeViewItem;
            TreeView.selectedIndicesChanged += OnSelectionChanged;

            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);
        }

        private void OnSelectionChanged(IEnumerable<int> selectedIndices)
        {
            if (!selectedIndices.Any())
                return;

            DialogueTreeItem sampleItem = TreeView.GetItemDataForIndex<DialogueTreeItem>(selectedIndices.First());

            if (sampleItem == null)
            {
                return;
            }

            OnSituationSelected?.Invoke(sampleItem.id);
        }
    }
}
