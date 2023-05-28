using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Editor.Graph.Utilities
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
