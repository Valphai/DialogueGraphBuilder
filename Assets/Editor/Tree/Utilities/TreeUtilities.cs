using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Saving;

namespace Chocolate4.Editor.Tree.Utilities
{
    public static class TreeUtilities
    {
        public static void ForceRefresh(TreeView treeView, Action<IEnumerable<int>> onSelectionChanged)
        {
            treeView.Rebuild();

            if (treeView.GetTreeCount() <= 0)
            {
                return;
            }

            // Force TreeView to call onSelectionChanged when it restores its own selection from view data.
            treeView.schedule.Execute(() => {
                onSelectionChanged(treeView.selectedIndices);
            });
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
