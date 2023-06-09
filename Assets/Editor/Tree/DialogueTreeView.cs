using static Chocolate4.DialogueEditorWindow;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Utilities;
using System.Linq;
using Chocolate4.Saving;
using Chocolate4.Editor.Saving;
using Chocolate4.Editor;
using Chocolate4.Editor.Tree.Utilities;
using UnityEditor;

namespace Chocolate4.Tree
{
    [Serializable]
    public class DialogueTreeView : IRebuildable<TreeSaveData>
    {
        public TreeView TreeView { get; private set; }

        public event Action<string> OnSituationSelected;
        public event Action<string> OnTreeItemRemoved;

        public void Initialize(TreeSaveData treeSaveData)
        {
            Rebuild(treeSaveData);
        }

        public void Rebuild(TreeSaveData treeSaveData)
        {
            TreeView = new TreeView();
            TreeView.reorderable = true;

            var items = new List<TreeViewItemData<DialogueTreeItem>>();

            IEnumerable<TreeItemSaveData> rootDatas = 
                treeSaveData.treeItemData.Where(itemSaveData => itemSaveData.depth == 0);

            int rootElementCount = rootDatas.Count();
            for (int i = 0; i < rootElementCount; i++)
            {
                TreeItemSaveData rooitItemSaveData = rootDatas.ElementAt(i);
                items.Add(
                    new TreeViewItemData<DialogueTreeItem>(i, rooitItemSaveData.rootItem,
                        TreeUtilities.GetChildren(treeSaveData, rooitItemSaveData, rootElementCount + 1)
                    )
                );
            }
            TreeView.SetRootItems(items);

            if (!items.IsNullOrEmpty())
            {
                TreeView.SetSelection(0);
            }

            CreateTreeView();
        }

        public TreeSaveData Save()
        {
            return StructureSaver.SaveTree(TreeView);
        }

        public void RemoveTreeItem(DialogueTreeItem item, int index)
        {
            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);

            int id = TreeView.GetIdForIndex(index);
            if (!TreeView.TryRemoveItem(id))
            {
                return;
            }

            OnTreeItemRemoved?.Invoke(item.guid);
        }

        public DialogueTreeItem AddTreeItem(string defaultName, TreeGroups treeGroup, TreeItemType elementType, int index = -1, string guidOverride = "")
        {
            int groupID = TreeView.GetIdForIndex(index);

            DialogueTreeItem treeItem = new DialogueTreeItem(defaultName, treeGroup.GetString(elementType), _ => new VisualElement());
            AddItemToGroup(treeItem, groupID);

            if (!guidOverride.Equals(string.Empty))
            {
                treeItem.guid = guidOverride;
            }

            return treeItem;
        }

        private void AddItemToGroup(DialogueTreeItem treeItem, int groupID)
        {
            int parentId = GUID.Generate().GetHashCode();
            TreeView.AddItem(
                new TreeViewItemData<DialogueTreeItem>(parentId, treeItem), groupID
            );

            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);
            TreeView.SetSelection(TreeView.viewController.GetIndexForId(parentId));
        }

        private void BindTreeViewItem(VisualElement element, int index)
        {
            DialogueTreeItem item = TreeView.GetItemDataForIndex<DialogueTreeItem>(index);


            VisualElement visualElement = element.ElementAt(0);
            VisualElement displayNameLabel = element.ElementAt(1);

            (visualElement as Label).text = item.prefix;
            Label renamableLabel = displayNameLabel as Label;
            renamableLabel.text = item.displayName;

            element.AddContextualMenu("Rename", _ => 
                VisualElementBuilder.Rename(renamableLabel, finishedText => {
                    item.displayName = finishedText;
                })
            );
            
            element.AddContextualMenu("Remove", _ => RemoveTreeItem(item, index));

            element.AddContextualMenu("Add Situation", _ => AddTreeItem(
                TreeGroupsExtensions.DefaultSituationName, TreeGroups.Situation, TreeItemType.Item, index)
            );
                    
            element.AddContextualMenu("Add Variable", _ => AddTreeItem(
                TreeGroupsExtensions.DefaultVariableName, TreeGroups.Variable, TreeItemType.Item, index)
            );

            element.AddContextualMenu("Add Variable Group", _ => AddTreeItem(
                TreeGroupsExtensions.DefaultVariableGroupName, TreeGroups.Variable, TreeItemType.Group, index)
            );

            element.AddContextualMenu("Add Event", _ => AddTreeItem(
                TreeGroupsExtensions.DefaultEventName, TreeGroups.Event, TreeItemType.Item, index)
            );

            element.AddContextualMenu("Add Event Group", _ => AddTreeItem(
                TreeGroupsExtensions.DefaultEventGroupName, TreeGroups.Event, TreeItemType.Group, index)
            );
        }

        private VisualElement MakeTreeViewItem()
        {
            VisualElement box = new VisualElement();
            box.AddToClassList(ContainerStyle);

            Label label = new Label();
            Label textLabel = new Label();

            box.Add(label);
            box.Add(textLabel);
            return box;
        }

        private void CreateTreeView()
        {
            TreeView.viewDataKey = "dialogue-tree";
            TreeView.fixedItemHeight = 20;

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

            if (sampleItem.prefix == TreeGroups.Situation.GetString(TreeItemType.Group))
            {
                OnSituationSelected?.Invoke(sampleItem.guid);
            }
        }

        public void GraphView_OnSituationCached(string situationGuid)
        {
            int count = TreeView.GetTreeCount();
            for (int i = 0; i < count; i++)
            {
                DialogueTreeItem item = TreeView.GetItemDataForIndex<DialogueTreeItem>(i);

                if (item.guid.Equals(situationGuid))
                {
                    return;
                }
            }

            AddTreeItem(
                TreeGroupsExtensions.DefaultSituationName, TreeGroups.Situation, 
                TreeItemType.Item, guidOverride:situationGuid
            );
        }
    }
}
