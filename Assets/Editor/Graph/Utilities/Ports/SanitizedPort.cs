using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger;
using Chocolate4.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class SanitizedPort : Port
    {
        public SanitizedPort(
            Orientation portOrientation, Direction portDirection, 
            Capacity portCapacity, Type type
        ) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static new SanitizedPort Create<TEdge>(Orientation orientation, Direction direction, Capacity capacity, Type type) where TEdge : Edge, new()
        {
            Port port = Port.Create<Edge>(orientation, direction, capacity, type);

            SanitizedPort sanitizedPort = new SanitizedPort(orientation, direction, capacity, type)
            {
                m_EdgeConnector = port.edgeConnector
            };

            sanitizedPort.AddManipulator(sanitizedPort.m_EdgeConnector);
            return sanitizedPort;
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);

            if (portName.Equals(NodeConstants.PropertyInput))
            {
                ((IPropertyNode)node).HideInputField();
            }

            SanitizeBackwardTransitions();
            SanitizeForwardTransitions(this);
        }

        public override void Disconnect(Edge edge)
        {
            Port disconnectedPort = edge.input == this ? edge.output : edge.input;

            if (portName.Equals(NodeConstants.PropertyInput))
            {
                ((IPropertyNode)node).DisplayInputField();
            }

            SanitizeBackwardTransitions();
            SanitizeForwardTransitions(this, (node) => {
                if (node == disconnectedPort.node)
                {
                    base.Disconnect(edge);
                    disconnectedPort.Disconnect(edge);
                }
            });

            base.Disconnect(edge);
            disconnectedPort.Disconnect(edge);
        }

        private void SanitizeForwardTransitions(Port startPort, Action<BaseNode> onConnectedPortsCached = null)
        {
            BaseNode thisNode = (BaseNode)startPort.node;
            LinkedList<BaseNode> nodeQueue = new LinkedList<BaseNode>();
            nodeQueue.AddLast(thisNode);

            LinkedListNode<BaseNode> evaluatedNode = nodeQueue.First;

            while (evaluatedNode != null)
            {
                BaseNode evaluatedBaseNode = evaluatedNode.Value;
                List<Port> evaluatedNodeOutputPorts = evaluatedBaseNode.outputContainer.Query<Port>().ToList();

                foreach (var outputPort in evaluatedNodeOutputPorts)
                {
                    List<Port> nextConnectedInputPorts = outputPort.ConnectedPorts().ToList();
                    onConnectedPortsCached?.Invoke(evaluatedBaseNode); 
                    foreach (var port in nextConnectedInputPorts)
                    {
                        BaseNode portOwner = (BaseNode)port.node;

                        Port outPort = portOwner.outputContainer.Q<Port>();
                        if (outPort == null)
                        {
                            continue;
                        }

                        nodeQueue.AddAfter(evaluatedNode, portOwner);

                        if (port is SanitizedPort sanitizedPort)
                        {
                            sanitizedPort.SanitizeBackwardTransitions();
                        }
                    }
                }

                LinkedListNode<BaseNode> previousInQueue = evaluatedNode;
                evaluatedNode = evaluatedNode.Next;
                nodeQueue.Remove(previousInQueue);
            }
        }

        private void SanitizeBackwardTransitions()
        {
            //if (startPort.portType != typeof(TransitionPortType))
            //{
            //    return;
            //}

            BaseNode thisNode = (BaseNode)node;
            LinkedList<BaseNode> nodeQueue = new LinkedList<BaseNode>();
            nodeQueue.AddLast(thisNode);

            LinkedListNode<BaseNode> evaluatedNode = nodeQueue.First;

            while (evaluatedNode != null)
            {
                BaseNode evaluatedBaseNode = evaluatedNode.Value;
                List<Port> evaluatedNodeInputPorts = evaluatedBaseNode.inputContainer.Query<Port>().ToList();

                foreach (var inputPort in evaluatedNodeInputPorts)
                {
                    List<Port> nextConnectedOutputPorts = inputPort.ConnectedPorts().ToList();
                    foreach (var port in nextConnectedOutputPorts)
                    {
                        BaseNode portOwner = (BaseNode)port.node;

                        Port inPort = portOwner.inputContainer.Q<Port>();
                        if (inPort == null)
                        {
                            continue;
                        }

                        nodeQueue.AddAfter(evaluatedNode, portOwner);

                        Port outPort = portOwner.outputContainer.Q<Port>(NodeConstants.TransferOut);
                        if (outPort == null)
                        {
                            continue;
                        }

                        if (outPort.IsConnectedTo(evaluatedBaseNode))
                        {
                            continue;
                        }

                        if (outPort.IsConnectedToAny())
                        {
                            // mark dangerous, log warning, disallow saving
                            Debug.Log($"{thisNode} detected previous transition to another branch on {portOwner}.");
                            HideTransitionPorts();
                            return;
                        }

                        //Debug.Log($"Not connected to any: {portOwner}");
                    }
                }

                LinkedListNode<BaseNode> previousInQueue = evaluatedNode;
                evaluatedNode = evaluatedNode.Next;
                nodeQueue.Remove(previousInQueue);
            }

            DisplayTransitionPorts();
        }
        
        private void DisplayTransitionPorts()
        {
            BaseNode dangerCauser = (BaseNode)node;
            if (dangerCauser.IsMarkedDangerous)
            {
                DangerLogger.UnmarkNodeDangerous(dangerCauser);
                return;
            }

            bool alreadyDrawn = node.inputContainer.Q<Port>(NodeConstants.TransferIn) != null;
            if (alreadyDrawn)
            {
                return;
            }

            ((IPropertyNode)node).DisplayTransitionPorts();  
        }

        private void HideTransitionPorts()
        {
            Port outputTransitionPort = node.outputContainer.Q<Port>(NodeConstants.TransferOut);
            bool alreadyHidden = outputTransitionPort == null;
            if (alreadyHidden)
            {
                return;
            }

            if (outputTransitionPort.IsConnectedToAny())
            {
                DangerLogger.MarkNodeDangerous($"{node} detected multiple previous routes! Make sure there is only one transition path for a branch.", (BaseNode)node);
                return;
            }

            Debug.Log($"Closing transitions.");
            ((IPropertyNode)node).HideTransitionPorts();
        }
    }
}