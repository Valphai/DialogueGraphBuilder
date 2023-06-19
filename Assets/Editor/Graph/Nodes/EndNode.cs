using Chocolate4.Dialogue.Edit.Utilities;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class EndNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Transfer;
        public override string Name { get; set; } = "End Node";

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return true;
        }
        
        protected override void DrawTitle()
        {
            base.DrawTitle();
            titleContainer.WithTransferStyle();
        }

        protected override void DrawOutputPort()
        {
            
        }
    }
}
