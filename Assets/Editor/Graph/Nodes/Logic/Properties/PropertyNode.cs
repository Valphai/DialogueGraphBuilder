using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class PropertyNode<T> : BaseNode, IPropertyNode
    {
        protected ConstantPortInput constantPortInput;

        public override NodeTask NodeTask { get; set; } = NodeTask.Property;
        public string PropertyID { get; internal set; }
        public T Value { get; internal set; }
        public bool IsBoundToProperty => !string.IsNullOrEmpty(PropertyID);
        public abstract PropertyType PropertyType { get; protected set; }

        protected abstract ConstantPortInput CreateConstantPortInput();

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new PropertyNodeSaveData()
            {
                nodeSaveData = saveData,
                propertyID = PropertyID,
                value = Value.ToString(),
                propertyType = PropertyType,
            };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            PropertyNodeSaveData propertySaveData = (PropertyNodeSaveData)saveData;
            PropertyID = propertySaveData.propertyID;
            PropertyType = propertySaveData.propertyType;

            UpdateLabel();
            if (IsBoundToProperty)
            {
                return;
            }

            Value = (T)Convert.ChangeType(propertySaveData.value, typeof(T));
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
            PropertyID = string.Empty;
            DisplayInputField();
            UpdateLabel();
        }

        public virtual void BindToProperty(IDialogueProperty property)
        {
            Name = property.DisplayName;
            PropertyID = property.Id;
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
            label.WithFontSize(UIStyles.FontSize)
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