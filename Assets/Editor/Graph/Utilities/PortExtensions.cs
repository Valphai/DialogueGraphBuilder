using System.Linq;

namespace UnityEditor.Experimental.GraphView
{
    public static class PortExtensions
    {
        public static bool IsConnectedTo(this Port port, Port another, Direction anotherPortType)
        {
            return anotherPortType == Direction.Input
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