using System.Linq;
using static Chocolate4.Edit.Graph.Utilities.NodeUtilities;

namespace UnityEditor.Experimental.GraphView
{
    public static class PortExtensions
    {
        public static bool IsConnectedTo(this Port port, Port another, PortType anotherPortType)
        {
            return anotherPortType == PortType.Input
                ? port.connections.Any(edge => edge.input == another)
                : port.connections.Any(edge => edge.output == another);
        }
        
        public static bool IsConnectedTo(this Port port, Port another)
        {
            return port.connections.Any(edge => edge.input == another)
                || port.connections.Any(edge => edge.output == another);
        }

        public static bool IsConnectedTo(this Port port, Node node)
        {
            return port.connections.Any(edge => edge.output?.node == node)
                || port.connections.Any(edge => edge.input?.node == node);
        }
    }
}