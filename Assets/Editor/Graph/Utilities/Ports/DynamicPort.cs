using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class DynamicPort : Port
    {
        public Action onDisconnect;
        public Action<Edge> onConnect;
        private bool isConstantViewDrawn;

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
            onConnect?.Invoke(edge);
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            onDisconnect?.Invoke();
        }

        public void TryDisplayConstantView()
        {
            if (isConstantViewDrawn)
            {
                return;
            }

            if (connected)
            {
                return;
            }

            if (portType == typeof(AnyValuePortType))
            {
                return;
            }

            isConstantViewDrawn = true;
            ((IConstantViewDraw)node).ConstantViewDrawer.DisplayConstantView(this);
        }

        public void TryHideConstantView()
        {
            if (!isConstantViewDrawn)
            {
                return;
            }

            isConstantViewDrawn = false;
            ((IConstantViewDraw)node).ConstantViewDrawer.HideConstantView(this);
        }
    }
}