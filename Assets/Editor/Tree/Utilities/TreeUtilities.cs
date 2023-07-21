using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Chocolate4.Dialogue.Edit.Tree.Utilities
{
    public static class TreeUtilities
    {
        public static Texture2D GetSituationIcon()
        {
            return (Texture2D)EditorGUIUtility.Load(FilePathConstants.GetEditorVisualAssetPath(FilePathConstants.situationIconPath));
        }

        public static void ForceRefresh(TreeView treeView, Action<IEnumerable<int>> onSelectionChanged)
        {
            treeView.Rebuild();

            if (treeView.viewController == null || treeView.GetTreeCount() <= 0)
            {
                return;
            }

            // Force TreeView to call onSelectionChanged when it restores its own selection from view data.
            treeView.schedule.Execute(() => {
                onSelectionChanged(treeView.selectedIndices);
            });
        }

        public static List<TreeViewItemData<DialogueTreeItem>> GetChildren(
            TreeSaveData treeSaveData, TreeItemSaveData treeItemSaveData, int nextId
        )
        {
            if (treeItemSaveData.childrenGuids.IsNullOrEmpty())
            {
                return null;
            }

            int count = treeItemSaveData.childrenGuids.Count;
            var children = new List<TreeViewItemData<DialogueTreeItem>>();
            int childStartingId = nextId + count;
            for (int i = 0; i < count; i++)
            {
                string childGuid = treeItemSaveData.childrenGuids[i];
                TreeItemSaveData childItemSaveData = treeSaveData.treeItemData.Find(itemData => itemData.rootItem.id == childGuid);
                DialogueTreeItem childItem = childItemSaveData.rootItem;

                children.Add(
                    new TreeViewItemData<DialogueTreeItem>(
                        nextId + i, 
                        childItem, 
                        GetChildren(treeSaveData, childItemSaveData, childStartingId)
                    )
                );
            }

            return children;
        }

        public static void FilterTreeViewBy(
            string value, TreeView treeView, Action<IEnumerable<int>> onSelectionChanged
        )
        {
            string filter = value.ToLower();

            List<DialogueTreeItem> displayedTreeItems = new List<DialogueTreeItem>();

            List<int> rootIds = treeView.GetRootIds().ToList();
            Dictionary<DialogueTreeItem, int> roots = 
                rootIds.ToDictionary(id => treeView.GetItemDataForId<DialogueTreeItem>(id));

            List<int> allIds = treeView.viewController.GetAllItemIds().ToList();
            Dictionary<DialogueTreeItem, int> allTreeItemIds = 
                allIds.ToDictionary(id => treeView.GetItemDataForId<DialogueTreeItem>(id));

            foreach (DialogueTreeItem root in roots.Keys)
            {
                var stack = new Stack<DialogueTreeItem>();
                stack.Push(root);
                while (stack.Count > 0)
                {
                    DialogueTreeItem current = stack.Pop();

                    if (current.displayName.ToLower().Contains(filter))
                    {
                        displayedTreeItems.Add(current);
                    }

                    IEnumerable<int> childrenIds = 
                        treeView.GetChildrenIdsForIndex(treeView.viewController.GetIndexForId(allTreeItemIds[current]));

                    List<DialogueTreeItem> children = childrenIds
                        .Select(childId => treeView.GetItemDataForId<DialogueTreeItem>(childId))
                        .Where(child => child != current)
                        .ToList();

                    if (children.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach (DialogueTreeItem child in children)
                    {
                        stack.Push(child);
                    }
                }
            }

            displayedTreeItems.Sort((a, b) => EditorUtility.NaturalCompare(a.displayName, b.displayName));

            treeView.SetRootItems(displayedTreeItems.Select(treeItem => new TreeViewItemData<DialogueTreeItem>(
                    allTreeItemIds[treeItem],
                    treeItem
                )).ToList()
            );

            treeView.ClearSelection();
            treeView.SetSelection(0);
            ForceRefresh(treeView, onSelectionChanged);
        }
    }
}
