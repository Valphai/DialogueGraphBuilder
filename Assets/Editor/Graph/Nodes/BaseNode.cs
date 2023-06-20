using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Edit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class BaseNode : Node, ISaveableNode
    {
        public string NextNodeId { get; set; }
        public string GroupID { get; set; }
        public string ID { get; set; }
        public Type NodeType { get; set; }
        public List<PortData> InputPortDataCollection { get; private set; }
        public List<PortData> OutputPortDataCollection { get; private set; }
        public abstract string Name { get; set; }
        public abstract NodeTask NodeTask { get; set; }

        public abstract bool CanConnectTo(BaseNode node, Direction direction);

        public virtual void Initialize(Vector3 startingPosition)
        {
            ID = Guid.NewGuid().ToString();
            InputPortDataCollection = new List<PortData>();
            OutputPortDataCollection = new List<PortData>();
            NodeType = GetType();

            SetPosition(new Rect(startingPosition, Vector2.zero));
        }

        public virtual void PostInitialize()
        {
            CreatePortData();
        }

        public virtual IDataHolder Save()
        {
            return new NodeSaveData()
            {
                inputPortDataCollection = InputPortDataCollection,
                outputPortDataCollection = OutputPortDataCollection,
                nodeID = ID,
                nodeType = NodeType.ToString(),
                position = GetPosition().position,
                groupID = GroupID,
            };
        }

        public virtual void Load(IDataHolder saveData)
        {
            InputPortDataCollection = saveData.NodeData.inputPortDataCollection;
            OutputPortDataCollection = saveData.NodeData.outputPortDataCollection;
            ID = saveData.NodeData.nodeID;
            GroupID = saveData.NodeData.groupID;
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
                .WithFontSize(UIStyles.FontSize)
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithHorizontalGrow()
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
            VisualElement contentContainer = new VisualElement();

            AddExtraContent(contentContainer);

            extensionContainer.Add(contentContainer);
        }

        protected virtual void AddExtraContent(VisualElement contentContainer)
        {
            
        }

        protected virtual void DrawOutputPort()
        {
            Port outputPort = DrawPort("Output", Direction.Output, Port.Capacity.Single);
            outputContainer.Add(outputPort);
        }

        protected virtual void DrawInputPort()
        {
            Port inputPort = DrawPort("Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);
        }

        protected Port DrawPort(string name, Direction direction, Port.Capacity capacity)
        {
            Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(bool));
            port.portName = port.name = name;

            return port;
        }

        protected void CreatePortData()
        {
            CreatePortData(inputContainer, InputPortDataCollection);
            CreatePortData(outputContainer, OutputPortDataCollection);
        }

        private void CreatePortData(VisualElement container, List<PortData> dataCollection)
        {
            List<Port> inputPorts = container.Children().Where(port => port is Port).Select(port => (Port)port).ToList();
            foreach (Port port in inputPorts)
            {
                PortData portData = new PortData() { thisPortName = port.portName };
                dataCollection.Add(portData);
            }
        }
    }
}
