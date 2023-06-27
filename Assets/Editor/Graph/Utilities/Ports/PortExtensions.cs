using System.Collections.Generic;
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

        public static bool IsConnectedToAny(this Port port)
        {
            return port.direction == Direction.Input
                ? port.connections.Any(edge => edge.input != null)
                : port.connections.Any(edge => edge.output != null);
        }

        public static IEnumerable<Port> ConnectedPorts(this Port port)
        {
            return port.direction == Direction.Input
                ? port.connections.Where(edge => edge.output != null).Select(edge => edge.output)
                : port.connections.Where(edge => edge.input != null).Select(edge => edge.input);
        }
    }
}