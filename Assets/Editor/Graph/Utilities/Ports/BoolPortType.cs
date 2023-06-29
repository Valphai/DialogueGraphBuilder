using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class BoolPortType : ConstantPortType<bool>
    {
        public override Type PortType => typeof(BoolPortType);

        protected override ConstantPortInput CreateConstantPortInput()
        {
            constantViewGeneric = new BoolConstantView((value) => Value = value);
            ConstantPortInput = new ConstantPortInput(constantViewGeneric);
            return ConstantPortInput;
        }
    }
}