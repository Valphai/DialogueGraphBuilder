using Chocolate4.Dialogue.Runtime.Utilities;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class BoolPropertyNode : PropertyNode<bool>
    {
        public override string Name { get; set; } = "Bool";
        public override PropertyType PropertyType { get; protected set; } = PropertyType.Bool;

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            if (direction == Direction.Output)
            {
                return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Logic;
            }

            return node.NodeTask != NodeTask.Property;
        }

        protected override ConstantPortInput CreateConstantPortInput()
        {
            return new ConstantPortInput(new BoolConstantView(this));
        }
    }
}