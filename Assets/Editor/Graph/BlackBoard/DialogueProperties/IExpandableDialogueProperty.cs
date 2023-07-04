using Chocolate4.Dialogue.Edit.Graph.Nodes;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    public interface IExpandableDialogueProperty
    {
        IConstantViewControlCreator ToConstantView();
        void UpdateConstantView();
    }
}