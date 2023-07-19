using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Graph.Edit
{
    public class DataPort : Port, ISaveable<PortData>
    {
        internal PortData PortData { get; private set; }

        public DataPort(
            string portName, Orientation portOrientation, 
            Direction portDirection, Capacity portCapacity, Type type
        ) : base(portOrientation, portDirection, portCapacity, type)
        {
            this.portName = name = portName;

            PortData = new PortData()
            {
                thisPortName = portName,
                thisPortType = portType.ToString()
            };
        }

        public static DataPort Create<TEdge>(
            string portName, Orientation orientation, 
            Direction direction, Capacity capacity, Type type
        ) where TEdge : Edge, new()
        {
            Port port = Create<Edge>(orientation, direction, capacity, type);

            DataPort dataPort = new DataPort(portName, orientation, direction, capacity, type)
            {
                m_EdgeConnector = port.edgeConnector,
            };

            dataPort.AddManipulator(dataPort.m_EdgeConnector);
            port.RemoveFromHierarchy();
            return dataPort;
        }

        public PortData Save()
        {
            return new PortData(PortData);
        }

        public void Load(PortData saveData)
        {
            PortData = new PortData(saveData);
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);

            // otherNodeID and otherPortName has yet to be used for input ports.
            Port otherPort = edge.GetOtherPort(this);

            BaseNode otherNode = (BaseNode)otherPort.node;
            PortData.otherNodeID = otherNode.Id;
            PortData.otherPortName = otherPort.portName;
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);

            PortData.otherNodeID = string.Empty;
            PortData.otherPortName = string.Empty;
        }
    }
}
