using Chocolate4.Dialogue.Runtime.Utilities;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class IntegerPropertyNode : PropertyNode<int>
    {
        private ConstantViewGeneric<int> constantViewGeneric;

        public override string Name { get; set; } = "Integer";
        public override PropertyType PropertyType { get; protected set; } = PropertyType.Integer;

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
            constantViewGeneric = new IntegerConstantView(this);
            return new ConstantPortInput(constantViewGeneric);
        }

        protected override void UpdateConstantViewGenericControl(int value)
        {
            constantViewGeneric.UpdateControl(value);
        }
    }
}