using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ConditionNode : BaseNode
    {
        public override string Name { get; set; } = "Condition";

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

        protected override void DrawInputPort()
        {
            Port inputPort = DrawPort("Input", Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputContainer.Add(inputPort);
        }

        protected override void DrawOutputPort()
        {
            Port truePort = DrawPort("True", Direction.Output, Port.Capacity.Single, typeof(TransitionPortType));
            Port falsePort = DrawPort("False", Direction.Output, Port.Capacity.Single, typeof(TransitionPortType));

            outputContainer.Add(truePort);
            outputContainer.Add(falsePort);
        }
    }
}
