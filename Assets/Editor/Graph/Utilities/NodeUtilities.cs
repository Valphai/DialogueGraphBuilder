using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public static class NodeUtilities
    {
        public static List<BaseNode> GetConnections(Port port, Direction requestedPort, Action<Port> actionOnOtherPort = null)
        {
            List<Edge> connections = port.connections.ToList();

            var connectionsMap = new List<BaseNode>();

            foreach (Edge connection in connections)
            {
                Port otherPort = requestedPort == Direction.Input ? connection.input : connection.output;
                actionOnOtherPort?.Invoke(otherPort);

                connectionsMap.Add(otherPort.node as BaseNode);
            }
            return connectionsMap;
        }

        public static bool IsConnectedTo(this BaseNode node, BaseNode another)
        {
            IEnumerable<Port> inputPorts = node.inputContainer.Children().OfType<Port>();
            IEnumerable<Port> outputPorts = node.outputContainer.Children().OfType<Port>();

            IEnumerable<Port> allPorts = inputPorts.Concat(outputPorts);
            return allPorts.Any(port => port.IsConnectedTo(another));
        }

        internal static void ClearConnectedPortDataTo(Port outputPort)
        {
            if (outputPort.connections.IsNullOrEmpty())
            {
                return;
            }

            Edge edge = outputPort.connections.First();
            List<Port> connectedPorts = outputPort.ConnectedPorts().ToList();

            foreach (Port port in connectedPorts)
            {
                BaseNode connectedOtherNode = (BaseNode)port.node;

                IEnumerable<PortData> portDataToRemove =
                    connectedOtherNode.InputPortDataCollection.Where(portData => portData.otherPortName.Equals(outputPort.portName));
                if (!portDataToRemove.IsNullOrEmpty())
                {
                    PortData ToRemove = portDataToRemove.First();
                    ToRemove.otherPortName = ToRemove.otherNodeID = string.Empty;
                }

                port.Disconnect(edge);
                connectedOtherNode.RefreshPorts();
            }

            edge.RemoveFromHierarchy();
        }

        internal static bool IsConnectedAtAnyPointTo(this BaseNode node, BaseNode another)
        {
            if (node.IsConnectedTo(another))
            {
                return true;
            }

            bool isConnected = false;
            Func<BaseNode, bool> onEveryNextNode = portOwner => {

                isConnected = portOwner == another;
                return isConnected;
            };

            if (!FindMatchingNode(node, Direction.Input, onEveryNextNode))
            {
                FindMatchingNode(node, Direction.Output, onEveryNextNode);
            }

            return isConnected;
        }

        private static bool FindMatchingNode(
            BaseNode startNode, Direction direction, Func<BaseNode, bool> onEveryNextNode
        )
        {
            LinkedList<BaseNode> nodeQueue = new LinkedList<BaseNode>();
            nodeQueue.AddLast(startNode);

            LinkedListNode<BaseNode> evaluatedNode = nodeQueue.First;

            while (evaluatedNode != null)
            {
                BaseNode evaluatedBaseNode = evaluatedNode.Value;
                List<Port> evaluatedNodeInputPorts =  
                    (direction == Direction.Input ? evaluatedBaseNode.inputContainer : evaluatedBaseNode.outputContainer)
                    .Query<Port>().ToList();

                foreach (Port inputPort in evaluatedNodeInputPorts)
                {
                    List<Port> nextConnectedOutputPorts = inputPort.ConnectedPorts().ToList();
                    foreach (var port in nextConnectedOutputPorts)
                    {
                        BaseNode portOwner = (BaseNode)port.node;

                        Port inPort = 
                            (direction == Direction.Input ? portOwner.inputContainer : portOwner.outputContainer)
                            .Q<Port>();

                        if (inPort == null)
                        {
                            continue;
                        }

                        if(onEveryNextNode.Invoke(portOwner))
                        {
                            return true;
                        }

                        nodeQueue.AddAfter(evaluatedNode, portOwner);
                    }
                }

                LinkedListNode<BaseNode> previousInQueue = evaluatedNode;
                evaluatedNode = evaluatedNode.Next;
                nodeQueue.Remove(previousInQueue);
            }

            return false;
        }
    }
}
