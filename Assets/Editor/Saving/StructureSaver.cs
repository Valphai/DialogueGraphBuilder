using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace Chocolate4.Dialogue.Edit.Saving
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
                Port outputPort = node.outputContainer.Q<Port>();
                if (outputPort != null)
                {
                    var connectionsMap = NodeUtilities.GetConnections(outputPort, NodeUtilities.PortType.Input);

                    if (!connectionsMap.IsNullOrEmpty())
                    {
                        connectionsMap.ForEach(child => {
                            if (string.IsNullOrEmpty(node.NextNodeId))
                            {
                                node.NextNodeId = child.ID;
                                return;
                            }

                            if (!node.NextNodeId.Equals(child.ID))
                            {
                                node.NextNodeId = child.ID;
                                return;
                            }
                        });
                    }
                }

                return new NodeSaveData()
                {
                    nextNodeId = node.NextNodeId,
                    nodeID = node.ID,
                    nodeType = node.NodeType.ToString(),
                    text = node.Text,
                    position = node.GetPosition().position,
                    groupID = node.GroupID,
                };
            }
        }

        public static TreeSaveData SaveTree(TreeView treeView)
        {
            List<KeyValuePair<int, IEnumerable<int>>> deepestChildIds = GetNodesByDepth(treeView);

            List<TreeItemSaveData> resultData = new List<TreeItemSaveData>();
            TreeSaveData result = new TreeSaveData(resultData);

            foreach (var deepestChildId in deepestChildIds)
            {
                int parentId = deepestChildId.Key;
                TreeItemSaveData childSaveData = SaveChild(treeView, parentId);

                resultData.Add(childSaveData);
            }

            return result;
        }

        private static List<KeyValuePair<int, IEnumerable<int>>> GetNodesByDepth(TreeView treeView)
        {
            Dictionary<int, IEnumerable<int>> parentToChildrenID = new Dictionary<int, IEnumerable<int>>();

            int treeCount = treeView.GetTreeCount();

            for (int i = 0; i < treeCount; i++)
            {
                int nextId = treeView.GetIdForIndex(i);
                IEnumerable<int> childIds = treeView.viewController.GetChildrenIds(nextId);

                parentToChildrenID.Add(nextId, childIds);
            }

            var deepestChildIds = parentToChildrenID.ToList();
            deepestChildIds.Sort((a, b) => treeView.GetDepthOfItemById(a.Key).CompareTo(treeView.GetDepthOfItemById(b.Key)));
            return deepestChildIds;
        }

        private static TreeItemSaveData SaveChild(TreeView treeView, int parentId)
        {
            DialogueTreeItem parent = treeView.GetItemDataForId<DialogueTreeItem>(parentId);

            List<string> childrenGuids = new List<string>();
            IEnumerable<int> childrenIds = treeView.viewController.GetChildrenIds(parentId);
            if (!childrenIds.IsNullOrEmpty())
            {
                foreach (int childId in childrenIds)
                {
                    childrenGuids.Add(treeView.GetItemDataForId<DialogueTreeItem>(childId).guid);
                }
            }

            TreeItemSaveData itemSaveData = new TreeItemSaveData(parent, childrenGuids, treeView.GetDepthOfItemById(parentId));
            return itemSaveData;
        }
    }
}
