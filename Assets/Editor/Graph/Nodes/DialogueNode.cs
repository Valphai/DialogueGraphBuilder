using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class DialogueNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Dialogue;
        public override string Name { get; set; } = "Dialogue Name";

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Property;
        }
    }
}
