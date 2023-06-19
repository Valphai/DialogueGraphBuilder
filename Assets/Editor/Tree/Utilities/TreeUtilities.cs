using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Dialogue.Edit.Tree.Utilities
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

        public static List<TreeViewItemData<DialogueTreeItem>> GetChildren(TreeSaveData treeSaveData, TreeItemSaveData treeItemSaveData, int nextId)
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
                TreeItemSaveData childItemSaveData = treeSaveData.treeItemData.Find(itemData => itemData.rootItem.guid == childGuid);
                DialogueTreeItem childItem = childItemSaveData.rootItem;

                children.Add(new TreeViewItemData<DialogueTreeItem>(nextId + i, childItem, GetChildren(treeSaveData, childItemSaveData, childStartingId)));
            }

            return children;
        }
    }
}
