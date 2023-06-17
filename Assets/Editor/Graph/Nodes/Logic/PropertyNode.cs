using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class PropertyNode<T> : BaseNode, IPropertyNode
    {
        protected ConstantPortInput constantPortInput;

        public override NodeTask NodeTask { get; set; } = NodeTask.Property;
        public string PropertyName { get; internal set; }
        public string PropertyGuid { get; internal set; }
        public T Value { get; internal set; }
        protected bool IsPropertyBound => PropertyGuid != string.Empty;
        public abstract PropertyType PropertyType { get; }

        protected abstract ConstantPortInput CreateConstantPortInput();

        public virtual void UnbindFromProperty()
        {
            DisplayInputField();
        }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            Name = PropertyName;

            VisualElement element = this.Q<VisualElement>("node-border");
            if (element != null)
            {
                element.WithOverflow();
            }
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            constantPortInput = CreateConstantPortInput();
            constantPortInput.style.position = Position.Absolute;
            constantPortInput.visible = IsPropertyBound;
            constantPortInput.BringToFront();
            constantPortInput
                .WithMarginLeft(UIStyles.ConstantPortInputMarginLeft)
                .WithWidth(UIStyles.ConstantPortInputMinWidth)
                .WithMinHeight(UIStyles.ConstantPortInputMinHeight)
                .WithHorizontalGrow()
                .WithMarginTop(0f)
                .WithBackgroundColor(UIStyles.defaultDarkColor);

            inputContainer.Add(constantPortInput);
        }

        protected override void DrawTitle()
        {
            base.DrawTitle();
            titleContainer.WithPropertyStyle();
        }

        protected override void DrawInputPort()
        {
            Port inputPort = DrawPort("Input", Direction.Input, Port.Capacity.Single);
            inputContainer.Add(inputPort);
        }

        protected override void DrawContent()
        {
        }

        protected virtual void HideInputField()
        {
            constantPortInput.visible = false;
        }
        
        protected virtual void DisplayInputField()
        {
            constantPortInput.visible = true;
        }
    }
}