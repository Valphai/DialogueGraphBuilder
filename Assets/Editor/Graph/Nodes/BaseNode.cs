using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Edit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class BaseNode : Node, ISaveableNode
    {
        public string NextNodeId { get; set; }
        public string GroupID { get; set; }
        public string ID { get; set; }
        public Type NodeType { get; set; }
        public List<PortData> OutputPortDatas { get; private set; }
        public abstract string Name { get; set; }
        public abstract NodeTask NodeTask { get; set; }

        public abstract bool CanConnectTo(BaseNode node, Direction direction);

        public virtual void Initialize(Vector3 startingPosition)
        {
            ID = Guid.NewGuid().ToString();
            OutputPortDatas = new List<PortData>();
            NodeType = GetType();

            SetPosition(new Rect(startingPosition, Vector2.zero));
        }

        public virtual void PostInitialize()
        {

        }

        public virtual IDataHolder Save()
        {
            return new NodeSaveData()
            {
                outputPortDatas = OutputPortDatas,
                nodeID = ID,
                nodeType = NodeType.ToString(),
                position = GetPosition().position,
                groupID = GroupID,
            };
        }

        public virtual void Load(IDataHolder saveData)
        {
            OutputPortDatas = saveData.NodeData.outputPortDatas;
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

            PortData portData = new PortData() { thisPortName = outputPort.portName };
            OutputPortDatas.Add(portData);
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
    }
}
