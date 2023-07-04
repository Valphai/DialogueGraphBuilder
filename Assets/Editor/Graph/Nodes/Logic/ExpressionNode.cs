using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ExpressionNode : BaseNode
    {
        public override string Name { get; set; } = "Expression Node";
        public string Text { get; set; }

        protected override void DrawTitle()
        {
            Label Label = new Label() { text = Name };

            Label.WithFontSize(UIStyles.FontSize)
                .WithMarginTop(UIStyles.LogicMarginTop);

            titleContainer.Insert(0, Label);
            titleContainer.WithLogicStyle();
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            contentContainer
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithTextField(Text, evt => Text = evt.newValue)
                .WithMinHeight(UIStyles.SmallTextFieldHeight);
        }
    }
}
