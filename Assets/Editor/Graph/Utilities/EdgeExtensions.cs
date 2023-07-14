using Chocolate4.Dialogue.Edit.Graph.Utilities;
using System.Collections.Generic;

namespace UnityEditor.Experimental.GraphView
{
    public static class EdgeExtensions
    {
        public static List<Port> GetOtherPorts(this Edge edge, Port thisPort)
        {
            List<Port> ports = new List<Port>();

            if (edge.input == thisPort)
            {
                NodeUtilities.GetConnections(edge.output, Direction.Input, (otherPort) => ports.Add(otherPort));
            }
            else if (edge.output == thisPort)
            {
                NodeUtilities.GetConnections(edge.input, Direction.Output, (otherPort) => ports.Add(otherPort));
            }

            return ports;
        }
    }
}