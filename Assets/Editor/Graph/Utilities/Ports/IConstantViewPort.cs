using Chocolate4.Dialogue.Edit.Graph.Nodes;

namespace Chocolate4.Edit.Graph.Utilities
{
    public interface IConstantViewPort
    {
        string StringValue { get; }

        ConstantPortInput GetConstantPortInput();
        void UpdateConstantViewGenericControl(string value);
    }
}