using Chocolate4.Dialogue.Edit.Graph.Nodes;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class IntegerPortType : ConstantPortType<int>
    {
        private IntegerConstantView constantViewGeneric;

        protected override ConstantPortInput CreateConstantPortInput()
        {
            constantViewGeneric = new IntegerConstantView((value) => Value = value);
            ConstantPortInput = new ConstantPortInput(constantViewGeneric);
            return ConstantPortInput;
        }

        public override void UpdateConstantViewGenericControl(string value)
        {
            if (!int.TryParse(value, out int result))
            {
                return;
            }

            constantViewGeneric.UpdateControl(result);
        }
    }
}