using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Edit.Graph.Utilities
{
    public static class NodeUtilities
    {
        public static List<BaseNode> GetConnections(Port port)
        {
            IEnumerable<Edge> connections = port.connections;

            var connectionsMap = new List<BaseNode>();

            foreach (Edge connection in connections)
            {
                connectionsMap.Add(connection.output.node as BaseNode);
            }
            return connectionsMap;
        }
    }
}
