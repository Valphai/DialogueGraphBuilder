using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.UI.GridLayoutGroup;

namespace UnityEditor.Experimental.GraphView
{
    public static class PortUtilities
    {
        public static DynamicPort DrawDynamicPort(string name, Direction direction, Port.Capacity capacity, Type type)
        {
            DynamicPort port = DynamicPort.Create<Edge>(Orientation.Horizontal, direction, capacity, type);
            port.portName = port.name = name;

            return port;
        }

        public static void OnConnect(List<DynamicPort> dynamicPorts, DynamicPort connectingPort, Edge edge)
        {
            BaseNode owner = (BaseNode)connectingPort.node;

            Type newPortType = connectingPort.direction == Direction.Input ? edge.output.portType : edge.input.portType;
            if (newPortType != connectingPort.portType)
            {
                for (int i = 0; i < dynamicPorts.Count; i++)
                {
                    dynamicPorts[i].HideConstantView();
                }
            }

            connectingPort.portType = newPortType;
            for (int i = 0; i < dynamicPorts.Count; i++)
            {
                DynamicPort port = dynamicPorts[i];

                port.portType = newPortType;
                port.DrawConstantView();
                owner.InputPortDataCollection[i].thisPortType = port.portType.ToString();
            }
        }

        public static void OnDisconnect(List<DynamicPort> dynamicPorts, Type defaultPortType)
        {
            bool noneAreConnected = dynamicPorts.All(neighbour => !neighbour.connected);
            if (!noneAreConnected)
            {
                return;
            }

            for (int i = 0; i < dynamicPorts.Count; i++)
            {
                dynamicPorts[i].portType = defaultPortType;
                dynamicPorts[i].HideConstantView();
            }
        }

        public static void OnConnectSameType(DynamicPort connectingPort)
        {
            connectingPort.HideConstantView();
        }

        public static void OnDisconnectSameType(DynamicPort connectedPort)
        {
            connectedPort.DrawConstantView();
        }

        public static void SetDynamicPortsSameType(List<DynamicPort> dynamicPorts)
        {
            foreach (DynamicPort port in dynamicPorts)
            {
                if (!port.connected)
                {
                    port.DrawConstantView();
                }

                port.onDisconnect = () => OnDisconnectSameType(port);
                port.onConnect = (edge) => OnConnectSameType(port);
            }
        }

        public static void SetDynamicPorts(List<DynamicPort> dynamicPorts, Type defaultType)
        {
            foreach (DynamicPort port in dynamicPorts)
            {
                port.onDisconnect = () => OnDisconnect(dynamicPorts, defaultType);
                port.onConnect = (edge) => OnConnect(dynamicPorts, port, edge);
            }
        }
    }
}