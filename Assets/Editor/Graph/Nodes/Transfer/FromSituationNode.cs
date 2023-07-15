using Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class FromSituationNode : SituationTransferNode
    {
        public override string Name { get; set; } = "From Situation";

        protected override void DrawInputPort()
        {
        }

        protected override void SanitizeSelectedSituation()
        {
            DangerDetector.SanitizeTransferNodes(this, DialogueEditorWindow.Window.GraphView);
        }
    }
}
