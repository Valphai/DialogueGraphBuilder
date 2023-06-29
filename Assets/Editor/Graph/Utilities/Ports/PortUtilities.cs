using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

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
            for (int i = 0; i < dynamicPorts.Count; i++)
            {
                dynamicPorts[i].TryHideConstantView();
            }

            connectingPort.portType = newPortType;
            for (int i = 0; i < dynamicPorts.Count; i++)
            {
                DynamicPort port = dynamicPorts[i];

                port.portType = newPortType;
                port.TryDisplayConstantView();
                owner.InputPortDataCollection[i].thisPortType = port.portType.ToString();
            }
        }

        public static void OnDisconnect(List<DynamicPort> dynamicPorts, Type defaultPortType)
        {
            bool noneAreConnected = dynamicPorts.All(neighbour => !neighbour.connected);
            if (!noneAreConnected)
            {
                for (int i = 0; i < dynamicPorts.Count; i++)
                {
                    dynamicPorts[i].TryDisplayConstantView();
                }

                return;
            }
            
            for (int i = 0; i < dynamicPorts.Count; i++)
            {
                dynamicPorts[i].TryHideConstantView();
                dynamicPorts[i].portType = defaultPortType;
            }
        }

        public static void OnConnectSameType(DynamicPort connectingPort)
        {
            connectingPort.TryHideConstantView();
        }

        public static void OnDisconnectSameType(DynamicPort connectedPort)
        {
            connectedPort.TryDisplayConstantView();
        }

        public static void SetDynamicPortsSameType(List<DynamicPort> dynamicPorts)
        {
            foreach (DynamicPort port in dynamicPorts)
            {
                if (!port.connected)
                {
                    port.TryDisplayConstantView();
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