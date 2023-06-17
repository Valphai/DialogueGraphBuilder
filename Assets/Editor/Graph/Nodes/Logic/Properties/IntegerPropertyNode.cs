using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class IntegerPropertyNode : PropertyNode<int>
    {
        public override PropertyType PropertyType => PropertyType.Integer;

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            if (direction == Direction.Output)
            {
                return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Logic;
            }

            return node.NodeTask != NodeTask.Property;
        }

        public override void UnbindFromProperty()
        {
            Name = "Integer";
            DisplayInputField();

            PropertyGuid = string.Empty;
        }

        protected override ConstantPortInput CreateConstantPortInput()
        {
            return new ConstantPortInput(new IntegerConstantView(this));
        }
    }
}