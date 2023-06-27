using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class DynamicPort : Port
    {
        private EqualNode equalNode;
        private Port neighbouringInputPort;

        public DynamicPort(
            Orientation portOrientation, Direction portDirection,
            Capacity portCapacity, Type type
        ) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static new DynamicPort Create<TEdge>(Orientation orientation, Direction direction, Capacity capacity, Type type) where TEdge : Edge, new()
        {
            Port port = Port.Create<Edge>(orientation, direction, capacity, type);

            DynamicPort dynamicPort = new DynamicPort(orientation, direction, capacity, type)
            {
                m_EdgeConnector = port.edgeConnector
            };

            dynamicPort.AddManipulator(dynamicPort.m_EdgeConnector);
            return dynamicPort;
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);

            if (equalNode == null)
            {
                equalNode = (EqualNode)node;
                neighbouringInputPort = equalNode.inputContainer.Query<Port>().Where(port => port != this).First();
            }

            portType = neighbouringInputPort.portType = edge.output.portType;

            equalNode.InputPortDataCollection[0].thisPortType = portType.ToString();
            equalNode.InputPortDataCollection[1].thisPortType = neighbouringInputPort.portType.ToString();
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);

            if (!connected && !neighbouringInputPort.connected)
            {
                portType = neighbouringInputPort.portType = typeof(AnyValuePortType);
            }
        }
    }
}