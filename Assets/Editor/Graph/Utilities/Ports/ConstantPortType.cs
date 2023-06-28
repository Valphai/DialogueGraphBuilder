using Chocolate4.Dialogue.Edit.Graph.Nodes;

namespace Chocolate4.Edit.Graph.Utilities
{
    public abstract class ConstantPortType<T> : IConstantViewPort
    {
        public ConstantPortInput ConstantPortInput { get; protected set; }
        public T Value { get; protected set; }
        public string StringValue => Value.ToString();

        protected abstract ConstantPortInput CreateConstantPortInput();
        public abstract void UpdateConstantViewGenericControl(string value);

        public ConstantPortInput GetConstantPortInput()
        {
            return ConstantPortInput ?? CreateConstantPortInput();
        }
    }
}