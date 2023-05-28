using Chocolate4.Editor.Graph.Utilities;
using Chocolate4.Saving;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Editor.Saving
{
    public class StructureSaver
    {
        public static GraphSaveData SaveGraph(List<SituationSaveData> situationToData)
        {
            return new GraphSaveData()
            {
                situationSaveData = situationToData
            };
        }

        public static SituationSaveData SaveSituation(
            string situationGuid, UQueryState<GraphElement> graphElements
        )
        {
            List<NodeSaveData> nodeSaveDatas = new List<NodeSaveData>();

            graphElements.ForEach(element => {
                if (element is BaseNode node)
                {
                    nodeSaveDatas.Add(SaveNode(node));
                }
            });

            return new SituationSaveData(situationGuid, nodeSaveDatas);

            NodeSaveData SaveNode(BaseNode node)
            {
                Port inputPort = node.inputContainer.Q<Port>();
                var connectionsMap = NodeUtilities.GetConnections(inputPort);

                if (!connectionsMap.IsNullOrEmpty())
                {
                    connectionsMap.ForEach(parent => {
                        if (!node.InputIDs.Contains(parent.ID))
                        {
                            node.InputIDs.Add(parent.ID);
                        }
                    });
                }

                return new NodeSaveData(node);
            }
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
