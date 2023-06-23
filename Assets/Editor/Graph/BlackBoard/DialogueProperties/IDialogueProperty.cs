using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    public interface IDialogueProperty : ISaveable<DialoguePropertySaveData>
    {
        string DisplayName { get; set; }
        PropertyType PropertyType { get; }
        string Id { get; }

        BaseNode ToConcreteNode();
    }
}