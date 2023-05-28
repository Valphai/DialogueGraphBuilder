using Chocolate4.Saving;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Chocolate4.Editor.Saving
{
    internal class StructureSaver
    {
        public static SituationSaveData SaveSituation(
            string situationGuid, Dictionary<BaseNode, List<BaseNode>> nodeToOtherNodes
        )
        {
            List<NodeSaveData> nodeSaveData = new List<NodeSaveData>();
            foreach (var pair in nodeToOtherNodes)
            {
                nodeSaveData.Add(new NodeSaveData(pair.Key, pair.Value));
            }

            return new SituationSaveData(situationGuid, nodeSaveData);
        }

        public static TreeSaveData SaveTree(TreeView treeView)
        {
            int treeItemsCount = treeView.GetTreeCount();
            IEnumerable<int> rootIds = treeView.GetRootIds();

            List<TreeItemSaveData> itemSaveData = new List<TreeItemSaveData>();
            TreeSaveData treeSaveData = new TreeSaveData(itemSaveData);

            for (int i = 0; i < treeItemsCount; i++)
            {
                int rootId = treeView.GetIdForIndex(i);
                if (!rootIds.Any(root => root == rootId))
                {
                    continue;
                }

                DialogueTreeItem rootItem = treeView.GetItemDataForId<DialogueTreeItem>(rootId);
                itemSaveData.Add(
                    GetChildren(rootItem, i, treeView)
                );
            }

            return treeSaveData;
        }

        private static TreeItemSaveData GetChildren(DialogueTreeItem parent, int parentIndex, TreeView treeView)
        {
            IEnumerable<int> childIds = treeView.GetChildrenIdsForIndex(parentIndex);

            var children = new List<TreeItemSaveData>();
            for (int j = 0; j < childIds.Count(); j++)
            {
                int childId = childIds.ElementAt(j);

                DialogueTreeItem child = treeView.GetItemDataForId<DialogueTreeItem>(childId);
                TreeItemSaveData childSaveData = GetChildren(child, parentIndex + j, treeView);
                children.Add(childSaveData);
            }

            return new TreeItemSaveData(parent, children);
        }
    }
}
