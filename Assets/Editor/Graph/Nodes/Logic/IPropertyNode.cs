using Chocolate4.Dialogue.Edit.Graph.BlackBoard;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public interface IPropertyNode
    {
        string PropertyName { get; }
        string PropertyGuid { get; }
        PropertyType PropertyType { get; }

        void UnbindFromProperty();
    }
}