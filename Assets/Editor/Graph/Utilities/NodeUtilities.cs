using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Edit.Graph.Utilities
{
    public static class NodeUtilities
    {
        public enum PortType
        {
            Input,
            Output
        }

        public static List<BaseNode> GetConnections(Port port, PortType requestedPort)
        {
            IEnumerable<Edge> connections = port.connections;

            var connectionsMap = new List<BaseNode>();

            foreach (Edge connection in connections)
            {
                connectionsMap.Add(
                    (requestedPort == PortType.Input ? connection.input.node : connection.output.node) as BaseNode
                );
            }
            return connectionsMap;
        }
    }
}
