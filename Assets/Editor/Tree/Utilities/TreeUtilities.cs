using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Saving;
using System.Linq;

namespace Chocolate4.Editor.Tree.Utilities
{
    public static class TreeUtilities
    {
        private const int TreeViewSelectionRestoreDelay = 1;
        private const int TreeViewInitialSelectionDelay = 2;

        public static void ForceRefresh(TreeView treeView, Action<IEnumerable<int>> onSelectionChanged)
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

        public static List<TreeViewItemData<DialogueTreeItem>> GetChildren(TreeItemSaveData treeItemSaveData, int nextId)
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
    }
}
