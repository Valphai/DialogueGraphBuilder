using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Runtime.Utilities;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class PropertyNode<T> : BaseNode, IPropertyNode
    {
        protected ConstantPortInput constantPortInput;

        public string PropertyId { get; internal set; }
        public T Value { get; internal set; }
        public bool IsBoundToProperty => !string.IsNullOrEmpty(PropertyId);
        public abstract PropertyType PropertyType { get; protected set; }

        protected abstract ConstantPortInput CreateConstantPortInput();
        protected abstract void UpdateConstantViewGenericControl(T value);
        
        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new PropertyNodeSaveData()
            {
                nodeSaveData = saveData,
                propertyID = PropertyId,
                value = Value.ToString(),
                propertyType = PropertyType,
            };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            PropertyNodeSaveData propertySaveData = (PropertyNodeSaveData)saveData;
            PropertyId = propertySaveData.propertyID;
            PropertyType = propertySaveData.propertyType;

            UpdateLabel();
            if (IsBoundToProperty)
            {
                return;
            }

            Value = (T)Convert.ChangeType(propertySaveData.value, typeof(T));
            UpdateConstantViewGenericControl(Value);
        }

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
                .WithBackgroundColor(UIStyles.DefaultDarkColor);

            inputContainer.Add(constantPortInput);
        }

        public virtual void UnbindFromProperty()
        {
            Name = PropertyType.ToString();
            PropertyId = string.Empty;
            DisplayInputField();
            UpdateLabel();
        }

        public virtual void BindToProperty(IDialogueProperty property)
        {
            Name = property.DisplayName;
            PropertyId = property.Id;
            HideInputField();
            UpdateLabel();
        }

        public void HideInputField()
        {
            constantPortInput.style.visibility = Visibility.Hidden;
        }
        
        public void DisplayInputField()
        {
            constantPortInput.style.visibility = Visibility.Visible;
        }

        public void HideTransitionPorts()
        {
            Port inputPort = inputContainer.Q<Port>(NodeConstants.TransferIn);
            Port outputPort = outputContainer.Q<Port>(NodeConstants.TransferOut);

            inputPort.RemoveFromHierarchy();
            outputPort.RemoveFromHierarchy();
        }

        public void DisplayTransitionPorts()
        {
            base.DrawInputPort();
            base.DrawOutputPort();
        }

        protected override void DrawTitle()
        {
            Label label = UpdateLabel();
            label.WithFontSize(UIStyles.FontSize)
                .WithMaxWidth(UIStyles.MaxWidth);

            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            titleContainer.WithPropertyStyle();
        }

        protected override void DrawInputPort()
        {
            Port inputPort = DrawPort(NodeConstants.PropertyInput, Direction.Input, Port.Capacity.Single, typeof(T));
            inputContainer.Add(inputPort);

            base.DrawInputPort();
        }

        protected override void DrawOutputPort()
        {
            Port outputPort = DrawPort(NodeConstants.PropertyOutput, Direction.Output, Port.Capacity.Single, typeof(T));
            outputContainer.Add(outputPort);

            base.DrawOutputPort();
        }

        protected override Port DrawPort(string name, Direction direction, Port.Capacity capacity, Type type)
        {
            SanitizedPort port = SanitizedPort.Create<Edge>(Orientation.Horizontal, direction, capacity, type);
            port.portName = port.name = name;

            return port;
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