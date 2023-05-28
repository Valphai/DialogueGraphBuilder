using Chocolate4.Saving;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4
{
    public abstract class BaseNode : Node
    {
        private const string ExtensionContainerUSS = "base-node__extension-container";
        private const string ContentContainerUSS = "base-node__content-container";
        private const string FilenameTextFieldUSS = "base-node__filename-textfield";
        private const string TextFieldHiddenUSS = "base-node__textfield__hidden";

        public List<string> InputIDs { get; set; } = new List<string>();
        public string GroupID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public Type NodeType { get; set; }

        public virtual void Initialize(Vector3 startingPosition)
        {
            ID = Guid.NewGuid().ToString();
            Name = "Dialogue name";
            Choices = new List<string>();
            Text = string.Empty;
            NodeType = GetType();

            SetPosition(new Rect(startingPosition, Vector2.zero));

            mainContainer.AddToClassList(ContentContainerUSS);
            extensionContainer.AddToClassList(ExtensionContainerUSS);
        }

        public virtual void Load(NodeSaveData saveData)
        {
            InputIDs = saveData.inputIDs;
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

        public virtual void DrawTitle()
        {
            TextField dialogueNameTextField = new TextField() {
                value = Name
            };

            dialogueNameTextField.AddToClassList(TextFieldHiddenUSS);
            dialogueNameTextField.AddToClassList(FilenameTextFieldUSS);

            titleContainer.Insert(0, dialogueNameTextField);
        }

        public virtual void DrawPorts()
        {
            DrawInputPort();
            DrawOutputPort();
        }

        public virtual VisualElement DrawContent()
        {
            VisualElement contentContainer = new VisualElement();

            TextField textField = new TextField() {
                value = Text,
                multiline = true
            };

            contentContainer.AddToClassList(ContentContainerUSS);

            contentContainer.Add(textField);

            extensionContainer.AddToClassList(TextFieldHiddenUSS);
            extensionContainer.AddToClassList(FilenameTextFieldUSS);

            extensionContainer.Add(contentContainer);

            return contentContainer;
        }

        protected void DrawOutputPort()
        {
            Port port = DrawPort("Output", Direction.Output, Port.Capacity.Single);
            outputContainer.Add(port);
        }

        protected void DrawInputPort()
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
