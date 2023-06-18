using Chocolate4.Utilities;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class StartNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Transfer;
        public override string Name { get; set; } = "Start Node";

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return true;
        }

        protected override void DrawTitle()
        {
            base.DrawTitle();
            titleContainer.WithTransferStyle();
        }

        protected override void DrawInputPort()
        {

        }
    }
}
