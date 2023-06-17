using UnityEditor.Experimental.GraphView;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class PlayerOptionNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Dialogue;
        public override string Name { get; set; } = "Player Option";

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Property;
        }
    }
}
