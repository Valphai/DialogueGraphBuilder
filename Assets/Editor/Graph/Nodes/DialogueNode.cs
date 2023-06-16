using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class DialogueNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Dialogue;
    }
}
