using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public interface IPropertyNode
    {
        string PropertyID { get; }
        PropertyType PropertyType { get; }
        bool IsBoundToProperty { get; }

        void UnbindFromProperty();
        void BindToProperty(IDialogueProperty property);
    }
}