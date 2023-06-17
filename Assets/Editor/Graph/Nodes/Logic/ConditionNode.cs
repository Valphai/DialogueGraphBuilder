using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ConditionNode : BaseNode
    {
        public override string Name { get; set; } = "Condition";
        public override NodeTask NodeTask { get; set; } = NodeTask.Logic;

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            if (direction == Direction.Output)
            {
                return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Logic;
            }

            return node.NodeTask == NodeTask.Dialogue;
        }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            Choices.Add("True");
            Choices.Add("False");
        }

        protected override void DrawTitle()
        {
            Label Label = new Label() { text = Name };

            Label.WithFontSize(UIStyles.LogicFontSize);
            Label.WithMarginTop(UIStyles.LogicMarginTop);

            titleContainer.Insert(0, Label);
            titleContainer.WithLogicStyle();
        }

        protected override void DrawContent()
        {
        }

        protected override void DrawPorts()
        {
            DrawInputPort();

            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = choice;
                outputContainer.Add(choicePort);
            }
        }
    }
}
