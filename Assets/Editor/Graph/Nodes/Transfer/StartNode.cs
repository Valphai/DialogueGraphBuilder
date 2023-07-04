using Chocolate4.Dialogue.Edit.Utilities;
using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class StartNode : BaseNode
    {
        public override string Name { get; set; } = "Start Node";

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
