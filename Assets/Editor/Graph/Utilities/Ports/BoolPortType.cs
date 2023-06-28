using Chocolate4.Dialogue.Edit.Graph.Nodes;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class BoolPortType : ConstantPortType<bool>
    {
        private BoolConstantView constantViewGeneric;

        protected override ConstantPortInput CreateConstantPortInput()
        {
            constantViewGeneric = new BoolConstantView((value) => Value = value);
            ConstantPortInput = new ConstantPortInput(constantViewGeneric);
            return ConstantPortInput;
        }

        public override void UpdateConstantViewGenericControl(string value)
        {
            if (!bool.TryParse(value, out bool result))
            {
                return;
            }

            constantViewGeneric.UpdateControl(result);
        }
    }
}