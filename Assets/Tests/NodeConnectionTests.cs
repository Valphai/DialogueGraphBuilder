using Chocolate4.Dialogue.Edit;
using Chocolate4.Dialogue.Edit.CodeGeneration;
using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Runtime.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Tests
{
    public class NodeConnectionTests
    {
        private NodeAdapter nodeAdapter;
        private DialogueEditorWindow window;

        [SetUp]
        public void Setup()
        {
            DialogueEditorAsset dialogueAsset = ScriptableObject.CreateInstance<DialogueEditorAsset>();
            Writer writer = DialogueFileMenuItem.WriteDefaultContent();
            dialogueAsset.LoadFromJson(writer.buffer.ToString());
            nodeAdapter = new NodeAdapter();

            window = DialogueEditorWindow.OpenEditor(dialogueAsset, dialogueAsset.GetInstanceID());
        }

        [TearDown] 
        public void Teardown()
        {
            window.Close();
        }

        [Test]
        [TestCase(typeof(DialogueNode))]
        [TestCase(typeof(EndNode))]
        [TestCase(typeof(StartNode))]
        [TestCase(typeof(IntegerPropertyNode))]
        [TestCase(typeof(BoolPropertyNode))]
        public void Dialogue_Node_Connects_To(Type nodeType)
        {
            MakeNodes(typeof(DialogueNode), nodeType, out BaseNode thisNode, out BaseNode otherNode);

            if (nodeType == typeof(EndNode))
            {
                TransitionPortConnectsToOneWayNode(Direction.Input, thisNode, otherNode);
                return;
            }
            else if (nodeType == typeof(StartNode))
            {
                TransitionPortConnectsToOneWayNode(Direction.Output, thisNode, otherNode);
                return;
            }
            else if (nodeType == typeof(ConditionNode))
            {
                TransitionPortConnectsToConditionNode(thisNode, otherNode);
                return;
            }

            AssertInputConnectsToOtherOutput(thisNode, otherNode);
            AssertInputConnectsToOtherOutput(otherNode, thisNode);
        }

        [Test]
        [TestCase(typeof(BoolPropertyNode))]
        [TestCase(typeof(EqualNode))]
        [TestCase(typeof(ConditionNode))]
        public void Bool_Node_Connects_To(Type nodeType)
        {
            MakeNodes(typeof(BoolPropertyNode), nodeType, out BaseNode thisNode, out BaseNode otherNode);

            if (nodeType == typeof(ConditionNode))
            {
                AssertInputConnectsToOtherOutput(otherNode, thisNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
                return;
            }
            else if (nodeType == typeof(EqualNode))
            {
                AssertInputConnectsToOtherOutput(thisNode, otherNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
                AssertInputConnectsToOtherOutput(otherNode, thisNode, NodeConstants.Input1, NodeConstants.PropertyOutput);
                return;
            }

            AssertInputConnectsToOtherOutput(thisNode, otherNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
            AssertInputConnectsToOtherOutput(otherNode, thisNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
        }

        [Test]
        [TestCase(typeof(IntegerPropertyNode))]
        [TestCase(typeof(EqualNode))]
        [TestCase(typeof(OperatorNode))]
        public void Int_Node_Connects_To(Type nodeType)
        {
            MakeNodes(typeof(IntegerPropertyNode), nodeType, out BaseNode thisNode, out BaseNode otherNode);

            if (nodeType == typeof(IntegerPropertyNode))
            {
                AssertInputConnectsToOtherOutput(thisNode, otherNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
                AssertInputConnectsToOtherOutput(otherNode, thisNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
                return;
            }
            else if (nodeType == typeof(OperatorNode))
            {
                AssertInputConnectsToOtherOutput(thisNode, otherNode, NodeConstants.PropertyInput, NodeConstants.PropertyOutput);
            }

            AssertInputConnectsToOtherOutput(otherNode, thisNode, NodeConstants.Input1, NodeConstants.PropertyOutput);
        }

        private void MakeNodes(
            Type thisNodeType, Type otherNodeType,
            out BaseNode thisNode, out BaseNode otherNode
        )
        {
            thisNode = window.GraphView.CreateNode(Vector3.zero, thisNodeType);
            otherNode = window.GraphView.CreateNode(Vector3.zero, otherNodeType);

            return;
        }

        private void AssertInputConnectsToOtherOutput(
            BaseNode thisNode, BaseNode otherNode,
            string thisInputName = NodeConstants.TransferIn, string otherOutputName = NodeConstants.TransferOut
        )
        {
            Port thisPort = thisNode.inputContainer.Q<Port>(thisInputName);
            Port otherPort = otherNode.outputContainer.Q<Port>(otherOutputName);

            List<Port> compatiblePorts = window.GraphView.GetCompatiblePorts(thisPort, nodeAdapter);
            Assert.IsTrue(compatiblePorts.Contains(otherPort));
        }

        private void TransitionPortConnectsToOneWayNode(Direction oneWayDirection, BaseNode thisNode, BaseNode otherNode)
        {
            switch (oneWayDirection)
            {
                case Direction.Input:
                    AssertInputConnectsToOtherOutput(otherNode, thisNode);
                    break;
                case Direction.Output:
                    AssertInputConnectsToOtherOutput(thisNode, otherNode);
                    break;
                default:
                    break;
            }
        }

        private void TransitionPortConnectsToConditionNode(BaseNode thisNode, BaseNode otherNode)
        {
            AssertInputConnectsToOtherOutput(
                thisNode, otherNode,
                otherOutputName: NodeConstants.ConditionOutputFalse
            );
            AssertInputConnectsToOtherOutput(
                thisNode, otherNode,
                otherOutputName: NodeConstants.ConditionOutputTrue
            );
        }
    }
}
