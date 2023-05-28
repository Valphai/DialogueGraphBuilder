using static Chocolate4.DialogueEditorWindow;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Utilities;
using System.Linq;
using Chocolate4.Saving;

namespace Chocolate4.Tree
{
    [Serializable]
    public class DialogueTreeView
    {
        private const int TreeViewSelectionRestoreDelay = 1;
        private const int TreeViewInitialSelectionDelay = 2;

        //private VisualElement contentPanel;

        public TreeView TreeView { get; private set; }

        public event Action<string> OnSituationSelected;

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
                    new TreeViewItemData<DialogueTreeItem>(i, rootElements.ElementAt(i), GetChildren(treeSaveData.treeItemData[i], rootElementCount + 1))
                );
            }
            TreeView.SetRootItems(items);

            CreateTreeView();
        }

        private static List<TreeViewItemData<DialogueTreeItem>> GetChildren(TreeItemSaveData treeItemSaveData, int nextId)
        {
            if (treeItemSaveData.children == null)
            {
                return null;
            }

            int count = treeItemSaveData.children.Count;
            var children = new List<TreeViewItemData<DialogueTreeItem>>();
            int childStartingId = nextId + count;
            for (int i = 0; i < count; i++)
            {
                var child = treeItemSaveData.children[i];
                children.Add(new TreeViewItemData<DialogueTreeItem>(nextId + i, treeItemSaveData.rootItem, GetChildren(child, childStartingId)));
            }

            return children;
        }

        public TreeSaveData SaveTree() 
        {
            return DialogueAssetManager.SaveTree(TreeView);
        }

        public void AddTreeItem(string defaultName, TreeGroups treeGroup, TreeItemType elementType, int groupID = -1)
        {
            AddItemToGroup(
                new DialogueTreeItem(defaultName, treeGroup.GetString(elementType), _ => new VisualElement()),
                groupID);
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
            TreeView.selectedIndicesChanged += OnSelectionChanged;

            TreeView.Rebuild();
            ForceRefresh(TreeView, OnSelectionChanged);
        }

        private void OnSelectionChanged(IEnumerable<int> selectedIndices)
        {
            if (!selectedIndices.Any())
                return;

            DialogueTreeItem sampleItem = TreeView.GetItemDataForIndex<DialogueTreeItem>(selectedIndices.First());

            //contentPanel.Clear();
            //contentPanel.Add(sampleItem.makeItem(sampleItem));

            if (sampleItem.prefix == TreeGroups.Situation.GetString(TreeItemType.Group))
            {
                OnSituationSelected?.Invoke(sampleItem.guid);
            }
        }

        private static void ForceRefresh(TreeView treeView, Action<IEnumerable<int>> onSelectionChanged)
        {
            // Force TreeView to call onSelectionChanged when it restores its own selection from view data.
            treeView.schedule.Execute(() => {
                onSelectionChanged(treeView.selectedIndices);
            }).StartingIn(TreeViewSelectionRestoreDelay);

            // Force TreeView to select something if nothing is selected.
            treeView.schedule.Execute(() => {
                if (treeView.selectedItems.Count() > 0)
                    return;

                treeView.SetSelection(0);

            }).StartingIn(TreeViewInitialSelectionDelay);
        }
    }
}
