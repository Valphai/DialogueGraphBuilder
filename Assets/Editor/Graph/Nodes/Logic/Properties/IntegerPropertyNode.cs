using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class IntegerPropertyNode : PropertyNode<int>
    {
        private ConstantViewGeneric<int> constantViewGeneric;

        public override string Name { get; set; } = "Integer Node";
        public override PropertyType PropertyType { get; protected set; } = PropertyType.Integer;
        protected override Type OutputPortType => typeof(IntegerPortType);

        protected override ConstantPortInput CreateConstantPortInput()
        {
            constantViewGeneric = new IntegerConstantView((value) => Value = value);
            return new ConstantPortInput(constantViewGeneric);
        }

        protected override void UpdateConstantViewGenericControl(int value)
        {
            constantViewGeneric.UpdateControl(value);
        }
    }
}