using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class IntegerPortType : ConstantPortType<int>
    {
        public override Type PortType => typeof(IntegerPortType);

        protected override ConstantPortInput CreateConstantPortInput()
        {
            constantViewGeneric = new IntegerConstantView((value) => Value = value);
            ConstantPortInput = new ConstantPortInput(constantViewGeneric);
            return ConstantPortInput;
        }
    }
}