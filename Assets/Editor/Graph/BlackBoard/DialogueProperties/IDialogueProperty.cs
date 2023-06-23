using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    public interface IDialogueProperty : ISaveable<DialoguePropertySaveData>, IHaveId
    {
        string DisplayName { get; set; }
        PropertyType PropertyType { get; }

        BaseNode ToConcreteNode();
        IConstantViewControlCreator ToConstantView();
        void UpdateConstantView();
    }
}