using Chocolate4.Dialogue.Runtime.Utilities;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class BoolPropertyNode : PropertyNode<bool>
    {
        private ConstantViewGeneric<bool> constantViewGeneric;

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
            constantViewGeneric = new BoolConstantView(this);
            return new ConstantPortInput(constantViewGeneric);
        }

        protected override void UpdateConstantViewGenericControl(bool value)
        {
            constantViewGeneric.UpdateControl(value);
        }
    }
}