using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class BaseNode : Node
    {
        public string NextNodeId { get; set; }
        public string GroupID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public Type NodeType { get; set; }
        public abstract NodeTask NodeTask { get; set; }

        public abstract bool CanConnectTo(BaseNode node, Direction direction);

        public virtual void Initialize(Vector3 startingPosition)
        {
            ID = Guid.NewGuid().ToString();
            Name = "Dialogue name";
            Choices = new List<string>();
            Text = string.Empty;
            NodeType = GetType();

            SetPosition(new Rect(startingPosition, Vector2.zero));
        }

        public virtual void Load(NodeSaveData saveData)
        {
            NextNodeId = saveData.nextNodeId;
            ID = saveData.nodeID;
            Text = saveData.text;
            GroupID = saveData.groupID;
        }

        public virtual void Draw()
        {
            DrawTitle();
            DrawPorts();
            DrawContent();

            RefreshExpandedState();
        }

        protected virtual void DrawTitle()
        {
            TextField textField = new TextField() { value = Name};

            textField.AddToClassList(UIStyles.TextFieldHiddenUSS);

            textField.Q<TextElement>()
                .WithFontSize(UIStyles.LogicFontSize)
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithExpandableHeight();

            titleContainer.Insert(0, textField);
            titleContainer.WithStoryStyle();
        }

        protected virtual void DrawPorts()
        {
            DrawInputPort();
            DrawOutputPort();
        }

        protected virtual void DrawContent()
        {
            VisualElement contentContainer = new VisualElement()
                .WithMinHeight(UIStyles.MaxHeight)
                .WithMaxWidth(UIStyles.MaxWidth);

            AddExtraContent(contentContainer);

            extensionContainer.Add(contentContainer);
        }

        protected virtual void AddExtraContent(VisualElement contentContainer)
        {
            TextField textField = new TextField()
            {
                value = Text,
                multiline = true,
            };
            textField.WithVerticalGrow()
                .WithStretchToParentHeight();

            contentContainer.Add(textField);

            textField.Q<TextElement>()
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithExpandableHeight();
        }

        protected virtual void DrawOutputPort()
        {
            Port port = DrawPort("Output", Direction.Output, Port.Capacity.Single);
            outputContainer.Add(port);
        }

        protected virtual void DrawInputPort()
        {
            Port port = DrawPort("Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(port);
        }

        protected Port DrawPort(string name, Direction direction, Port.Capacity capacity)
        {
            Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(bool));
            port.portName = name;
            return port;
        }
    }
}
