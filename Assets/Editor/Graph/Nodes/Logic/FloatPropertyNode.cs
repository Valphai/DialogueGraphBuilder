using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class FloatPropertyNode : BaseNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Value;

        public string PropertyGuid { get; internal set; }
        public float Value { get; internal set; }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            Name = "Float";
        }

        protected override void DrawInputPort()
        {
            Port port = DrawPort("Input", Direction.Input, Port.Capacity.Single);
            inputContainer.Add(port);
        }

        protected override void DrawContent()
        {
        }
    }
}