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
        public string PropertyGuid { get; internal set; }
        public T Value { get; internal set; }
        public bool IsBoundToProperty => !string.IsNullOrEmpty(PropertyGuid);
        public abstract PropertyType PropertyType { get; }

        protected abstract ConstantPortInput CreateConstantPortInput();

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            if (!IsBoundToProperty)
            {
                Name = PropertyType.ToString(); 
            }

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
            
            if (!IsBoundToProperty)
            {
                DisplayInputField();
            }
            else
            {
                HideInputField();
            }
            
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

        public virtual void UnbindFromProperty()
        {
            Name = PropertyType.ToString();
            PropertyGuid = string.Empty;
            DisplayInputField();
            UpdateLabel();
        }

        public virtual void BindToProperty(IDialogueProperty property)
        {
            Name = property.DisplayName;
            PropertyGuid = property.Guid;
            HideInputField();
            UpdateLabel();
        }

        protected virtual void HideInputField()
        {
            constantPortInput.style.visibility = Visibility.Hidden;
        }
        
        protected virtual void DisplayInputField()
        {
            constantPortInput.style.visibility = Visibility.Visible;
        }

        protected override void DrawTitle()
        {
            Label label = UpdateLabel();
            label.WithFontSize(UIStyles.LogicFontSize)
                .WithMaxWidth(UIStyles.MaxWidth);

            label.style.unityTextAlign = TextAnchor.MiddleCenter;

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

        private Label UpdateLabel()
        {
            Label label = titleContainer.Q<Label>();
            label.text = Name;

            return label;
        }
    }
}