using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using static Chocolate4.Edit.Graph.Utilities.NodeUtilities;

namespace Chocolate4.Dialogue.Edit.Saving
{
    public class StructureSaver
    {
        public static SituationSaveData SaveSituation(
            string situationGuid, UQueryState<GraphElement> graphElements
        )
        {
            List<IDataHolder> nodeSaveDatas = new List<IDataHolder>();

            graphElements.ForEach(element => {
                if (element is BaseNode node)
                {
                    nodeSaveDatas.Add(SaveNode(node));
                }
            });

            return new SituationSaveData(situationGuid, nodeSaveDatas);
        }

        public static IDataHolder SaveNode(BaseNode node)
        {
            List<Port> outputPorts = node.outputContainer.Query<Port>().ToList();
            List<Port> inputPorts = node.inputContainer.Query<Port>().ToList();

            if (!outputPorts.IsNullOrEmpty())
            {
                SaveConnections(node.OutputPortDataCollection, outputPorts, Direction.Input);
            }
            if (!inputPorts.IsNullOrEmpty())
            {
                SaveConnections(node.InputPortDataCollection, inputPorts, Direction.Output);
            }

            return node.Save();
        }

        public static TreeSaveData SaveTree(TreeView treeView)
        {
            List<KeyValuePair<int, List<int>>> deepestChildIds = GetNodesByDepth(treeView);

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

        private static void SaveConnections(List<PortData> portDataCollection, List<Port> ports, Direction requestedPort)
        {
            foreach (Port port in ports)
            {
                PortData portData = portDataCollection.Find(portData => portData.thisPortName.Equals(port.portName));

                List<BaseNode> connectedNodes = 
                    GetConnections(port, requestedPort, (otherPort) => portData.otherPortName = otherPort.portName);

                if (connectedNodes.IsNullOrEmpty())
                {
                    portData.otherNodeID = portData.otherPortName = string.Empty;
                    continue;
                }

                connectedNodes.ForEach(child => portData.otherNodeID = child.Id);
            }
        }

        private static List<KeyValuePair<int, List<int>>> GetNodesByDepth(TreeView treeView)
        {
            Dictionary<int, List<int>> parentToChildrenID = new Dictionary<int, List<int>>();

            int treeCount = treeView.GetTreeCount();

            for (int i = 0; i < treeCount; i++)
            {
                int nextId = treeView.GetIdForIndex(i);
                List<int> childIds = treeView.viewController.GetChildrenIds(nextId).ToList();

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
            List<int> childrenIds = treeView.viewController.GetChildrenIds(parentId).ToList();
            if (!childrenIds.IsNullOrEmpty())
            {
                foreach (int childId in childrenIds)
                {
                    childrenGuids.Add(treeView.GetItemDataForId<DialogueTreeItem>(childId).id);
                }
            }

            TreeItemSaveData itemSaveData = new TreeItemSaveData(parent, childrenGuids, treeView.GetDepthOfItemById(parentId));
            return itemSaveData;
        }
    }
}
