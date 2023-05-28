using Chocolate4.Editor.Graph.Nodes;
using UnityEngine;

namespace Chocolate4
{
    public class DialogueNode : BaseNode
    {
        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);
            NodeType = NodeTypes.DialogueNode;
        }
    }
}
