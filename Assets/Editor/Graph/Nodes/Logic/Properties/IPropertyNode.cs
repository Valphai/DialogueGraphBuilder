using Chocolate4.Dialogue.Edit.Graph.BlackBoard;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public interface IPropertyNode
    {
        string PropertyGuid { get; }
        PropertyType PropertyType { get; }
        bool IsBoundToProperty { get; }

        void UnbindFromProperty();
        void BindToProperty(IDialogueProperty property);
    }
}