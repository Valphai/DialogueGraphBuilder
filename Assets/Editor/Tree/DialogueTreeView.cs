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

            IEnumerable<DialogueTreeItem> rootElements = 
                treeSaveData.treeItemData.Select(itemSaveData => itemSaveData.rootItem);

            int rootElementCount = rootElements.Count();
            for (int i = 0; i < rootElementCount; i++)
            {
                items.Add(
                    new TreeViewItemData<DialogueTreeItem>(i, rootElements.ElementAt(i), 
                        TreeUtilities.GetChildren(treeSaveData.treeItemData[i], rootElementCount + 1)
                    )
                );
            }
            TreeView.SetRootItems(items);

            CreateTreeView();
        }

        public TreeSaveData Save()
        {
            return StructureSaver.SaveTree(TreeView);
        }

        public void RemoveTreeItem(DialogueTreeItem item, int id)
        {
            if (!TreeView.TryRemoveItem(id))
            {
                return;
            }

            OnTreeItemRemoved?.Invoke(item.guid);
        }

        public void AddTreeItem(string defaultName, TreeGroups treeGroup, TreeItemType elementType, int groupID = -1, string guidOverride = "")
        {
            DialogueTreeItem treeItem = new DialogueTreeItem(defaultName, treeGroup.GetString(elementType), _ => new VisualElement());
            AddItemToGroup(treeItem, groupID);

            if (!guidOverride.Equals(string.Empty))
            {
                treeItem.guid = guidOverride;
            }
        }

        private void AddItemToGroup(DialogueTreeItem treeItem, int groupID)
        {
            int treeItemCount = TreeView.GetTreeCount();
            int parentId = treeItemCount + 1;
            TreeView.AddItem(
                new TreeViewItemData<DialogueTreeItem>(parentId, treeItem), groupID
            );

            TreeView.Rebuild();
        }

        private void BindTreeViewItem(VisualElement element, int index)
        {
            DialogueTreeItem item = TreeView.GetItemDataForIndex<DialogueTreeItem>(index);
            VisualElement visualElement = element.ElementAt(0);
            VisualElement displayNameLabel = element.ElementAt(1);

            (visualElement as Label).text = item.prefix;
            Label renamableLabel = displayNameLabel as Label;
            renamableLabel.text = item.displayName;

            int groupID = TreeView.GetIdForIndex(index);

            element.AddContextualMenu("Rename", _ => 
                VisualElementBuilder.Rename(renamableLabel, finishedText => {
                    item.displayName = finishedText;
                })
            );
            
            element.AddContextualMenu("Remove", _ => RemoveTreeItem(item, groupID));


            switch (item.prefix)
            {
                case TreeGroupsExtensions.SituationString:
                    element.AddContextualMenu("Add Situation", _ => AddTreeItem(
                        TreeGroupsExtensions.DefaultSituationName, TreeGroups.Situation, TreeItemType.Item, groupID)
                    );
                    
                    break;

                case TreeGroupsExtensions.VariableGroupString:
                    element.AddContextualMenu("Add Variable", _ => AddTreeItem(
                        TreeGroupsExtensions.DefaultVariableName, TreeGroups.Variable, TreeItemType.Item, groupID)
                    );

                    element.AddContextualMenu("Add Variable Group", _ => AddTreeItem(
                        TreeGroupsExtensions.DefaultVariableGroupName, TreeGroups.Variable, TreeItemType.Group, groupID)
                    );
                    break;

                case TreeGroupsExtensions.EventGroupString:
                    element.AddContextualMenu("Add Event", _ => AddTreeItem(
                        TreeGroupsExtensions.DefaultEventName, TreeGroups.Event, TreeItemType.Item, groupID)
                    );

                    element.AddContextualMenu("Add Event Group", _ => AddTreeItem(
                        TreeGroupsExtensions.DefaultEventGroupName, TreeGroups.Event, TreeItemType.Group, groupID)
                    );
                    break;
                default:
                    break;
            }
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
            //TreeView.unbindItem = UnbindTreeViewItem;
            TreeView.selectedIndicesChanged += OnSelectionChanged;

            TreeView.Rebuild();
            TreeUtilities.ForceRefresh(TreeView, OnSelectionChanged);
        }

        //private void UnbindTreeViewItem(VisualElement element, int index)
        //{
        //}

        private void OnSelectionChanged(IEnumerable<int> selectedIndices)
        {
            if (!selectedIndices.Any())
                return;

            DialogueTreeItem sampleItem = TreeView.GetItemDataForIndex<DialogueTreeItem>(selectedIndices.First());

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
