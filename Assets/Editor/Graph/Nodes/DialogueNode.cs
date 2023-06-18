using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class DialogueNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Dialogue;
        public override string Name { get; set; } = "Dialogue Name";

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Property;
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            contentContainer
                .WithMinHeight(UIStyles.MaxHeight)
                .WithMaxWidth(UIStyles.MaxWidth);

            TextField textField = new TextField()
            {
                value = Text,
                multiline = true,
            };
            textField.WithVerticalGrow()
                .WithFlexGrow();

            contentContainer.Add(textField);

            textField.Q<TextElement>()
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithExpandableHeight();
        }
    }
}
