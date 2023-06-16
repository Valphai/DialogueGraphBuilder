using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class AdditionNode : BaseNode, ILogicEvaluate
    {
        private const string Port1 = "Input 1";
        private const string Port2 = "Input 2";

        public override NodeTask NodeTask { get; set; } = NodeTask.Logic;

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            Name = "Addition";
        }

        protected override void DrawInputPort()
        {
            Port port1 = DrawPort(Port1, Direction.Input, Port.Capacity.Single);
            Port port2 = DrawPort(Port2, Direction.Input, Port.Capacity.Single);

            inputContainer.Add(port1);
            inputContainer.Add(port2);
        }

        protected override void DrawTitle()
        {
            Label Label = new Label() { text = "Add" };
            Label.WithFontSize(UIStyles.LogicFontSize);
            Label.WithMarginTop(UIStyles.LogicMarginTop);

            titleContainer.Insert(0, Label);
            titleContainer.WithLogicStyle();
        }

        protected override void DrawContent()
        {
        }

        public void Evaluate()
        {
            Port port1 = inputContainer.Q<Port>(Port1);
            Port port2 = inputContainer.Q<Port>(Port2);

            BaseNode node1 = (BaseNode)port1.connections.First(edge => edge.output.node != this).output.node;
            BaseNode node2 = (BaseNode)port2.connections.First(edge => edge.output.node != this).output.node;

            if (node1.NodeType != node2.NodeType)
            {
                Debug.LogError($"Could not perform {node1.NodeType} + {node2.NodeType}");
                return;
            }

            if (node1 is FloatPropertyNode)
            {
                outputContainer.Q<Port>("Output").userData = ((FloatPropertyNode)node1).Value + ((FloatPropertyNode)node2).Value;
            }
        }
    }
}
