using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Edit.Graph.Utilities;
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

        protected override void DrawTitle()
        {
            Label Label = new Label() { text = Name };

            Label.WithFontSize(UIStyles.FontSize)
                .WithMarginTop(UIStyles.LogicMarginTop);

            titleContainer.Insert(0, Label);
            titleContainer.WithLogicStyle();
        }

        protected override void DrawContent()
        {
        }

        protected override void DrawPorts()
        {
            DrawInputPort();

            Port truePort = DrawPort("True", Direction.Output, Port.Capacity.Single);
            Port falsePort = DrawPort("False", Direction.Output, Port.Capacity.Single);

            outputContainer.Add(truePort);
            outputContainer.Add(falsePort);
        }
    }
}
