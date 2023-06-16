using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class PropertyNode<T> : BaseNode, IPropertyNode
    {
        public override NodeTask NodeTask { get; set; } = NodeTask.Property;

        public string PropertyGuid { get; internal set; }
        public T Value { get; internal set; }
        public abstract PropertyType PropertyType { get; }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            Name = typeof(T).ToString().Replace("System.", string.Empty);

            Name = Name == "Int32" ? "Integer" : Name;
        }

        protected override void DrawTitle()
        {
            base.DrawTitle();
            titleContainer.WithPropertyStyle();
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