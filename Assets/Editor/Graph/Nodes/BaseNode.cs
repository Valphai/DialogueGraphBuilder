using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Edit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Chocolate4.Runtime.Utilities;
using Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class BaseNode : Node, ISaveable<IDataHolder>, IHaveId, IDangerCauser
    {
        public string GroupId { get; set; }
        public string Id { get; set; }
        public Type NodeType { get; set; }
        public List<PortData> InputPortDataCollection { get; private set; }
        public List<PortData> OutputPortDataCollection { get; private set; }
        public abstract string Name { get; set; }
        public bool IsMarkedDangerous { get; set; }

        public virtual IDataHolder Save()
        {
            return new NodeSaveData()
            {
                inputPortDataCollection = InputPortDataCollection,
                outputPortDataCollection = OutputPortDataCollection,
                nodeId = Id,
                nodeType = NodeType.ToString(),
                position = GetPosition().position,
                groupId = GroupId,
            };
        }

        public virtual void Load(IDataHolder saveData)
        {
            InputPortDataCollection = saveData.NodeData.inputPortDataCollection;
            OutputPortDataCollection = saveData.NodeData.outputPortDataCollection;
            Id = saveData.NodeData.nodeId;
            GroupId = saveData.NodeData.groupId;

            LoadPortTypes(InputPortDataCollection, inputContainer);
            LoadPortTypes(OutputPortDataCollection, outputContainer);
        }

        public virtual void Initialize(Vector3 startingPosition)
        {
            Id = Guid.NewGuid().ToString();
            InputPortDataCollection = new List<PortData>();
            OutputPortDataCollection = new List<PortData>();
            NodeType = GetType();

            SetPosition(new Rect(startingPosition, Vector2.zero));
        }

        public virtual void PostInitialize()
        {
            CreatePortData();
        }

        public virtual void Draw()
        {
            DrawTitle();
            DrawPorts();
            DrawContent();

            RefreshExpandedState();
        }

        public virtual void RefreshNode(Port startPort)
        {

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
            Port outputPort = DrawPort(NodeConstants.TransferOut, Direction.Output, Port.Capacity.Single, typeof(TransitionPortType));
            outputContainer.Add(outputPort);
        }

        protected virtual void DrawInputPort()
        {
            Port inputPort = DrawPort(NodeConstants.TransferIn, Direction.Input, Port.Capacity.Multi, typeof(TransitionPortType));
            inputContainer.Add(inputPort);
        }

        protected virtual Port DrawPort(string name, Direction direction, Port.Capacity capacity, Type type)
        {
            Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, type);
            port.portName = port.name = name;

            return port;
        }

        protected virtual void CreatePortData()
        {
            CreatePortData(inputContainer, InputPortDataCollection);
            CreatePortData(outputContainer, OutputPortDataCollection);
        }

        private void CreatePortData(VisualElement container, List<PortData> dataCollection)
        {
            List<Port> ports = container.Children().Where(port => port is Port).Select(port => (Port)port).ToList();
            foreach (Port port in ports)
            {
                PortData portData = new PortData() 
                { 
                    thisPortName = port.portName,
                    thisPortType = port.portType.ToString()
                };
                dataCollection.Add(portData);
            }
        }

        private void LoadPortTypes(List<PortData> portDataCollection, VisualElement portContainer)
        {
            List<Port> ports = portContainer.Query<Port>().ToList();
            for (int i = 0; i < ports.Count; i++)
            {
                ports[i].portType = Type.GetType(portDataCollection[i].thisPortType);
            }
        }
    }
}
