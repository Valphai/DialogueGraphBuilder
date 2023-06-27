using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using NodeConstants = Chocolate4.Runtime.Utilities.NodeConstants;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class EqualNode : BaseNode
    {
        private PopupField<string> popupField;
        private EqualityType equalityTypeToUse;

        public override string Name { get; set; } = "Equality Node";

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new EqualityNodeSaveData() { equalityEnum = equalityTypeToUse, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            EqualityNodeSaveData equalitySaveData = (EqualityNodeSaveData)saveData;
            equalityTypeToUse = equalitySaveData.equalityEnum;
        }

        public override void RefreshNode(Port connectingPort)
        {
            base.RefreshNode(connectingPort);
        }

        protected override void DrawTitle()
        {
            Label Label = new Label() { text = Name };
            Label.WithFontSize(UIStyles.FontSize)
                .WithMarginTop(UIStyles.LogicMarginTop);

            titleContainer.Insert(0, Label);
            titleContainer.WithLogicStyle();
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            CreatePopup();
            contentContainer.Add(popupField);
        }

        protected override void DrawInputPort()
        {
            Port inputPort1 = DrawDynamicPort(NodeConstants.Input1, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));
            Port inputPort2 = DrawDynamicPort(NodeConstants.Input2, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));

            inputContainer.Add(inputPort1);
            inputContainer.Add(inputPort2);
        }

        protected override void DrawOutputPort()
        {
            Port outputPort = DrawPort(NodeConstants.PropertyOutput, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputContainer.Add(outputPort);
        }

        protected DynamicPort DrawDynamicPort(string name, Direction direction, Port.Capacity capacity, Type type)
        {
            DynamicPort port = DynamicPort.Create<Edge>(Orientation.Horizontal, direction, capacity, type);
            port.portName = port.name = name;

            return port;
        }

        private void CreatePopup()
        {
            List<string> choices = Enum.GetNames(typeof(EqualityType)).ToList();

            popupField = new PopupField<string>(choices, 0, selectedName => {
                equalityTypeToUse = (EqualityType)Enum.Parse(typeof(EqualityType), selectedName);
                return selectedName;
            });
        }
    }
}
