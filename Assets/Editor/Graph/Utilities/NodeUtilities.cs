using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Edit.Graph.Utilities
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
    }
}
