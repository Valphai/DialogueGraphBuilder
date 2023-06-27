using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class OperatorNode : BaseNode//, ILogicEvaluate
    {
        private PopupField<string> popupField;
        private OperatorType operatorToUse;

        public override string Name { get; set; } = "Operator";

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new OperatorNodeSaveData() { operatorEnum = operatorToUse, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            OperatorNodeSaveData operatorSaveData = (OperatorNodeSaveData)saveData;
            operatorToUse = operatorSaveData.operatorEnum;
        }

        protected override void DrawOutputPort()
        {
            Port outputPort = DrawPort(NodeConstants.PropertyOutput, Direction.Output, Port.Capacity.Single, typeof(int));
            outputContainer.Add(outputPort);
        }

        protected override void DrawInputPort()
        {
            Port inputPort1 = DrawPort(NodeConstants.Input1, Direction.Input, Port.Capacity.Single, typeof(int));
            Port inputPort2 = DrawPort(NodeConstants.Input2, Direction.Input, Port.Capacity.Single, typeof(int));

            inputContainer.Add(inputPort1);
            inputContainer.Add(inputPort2);
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

        private void CreatePopup()
        {
            List<string> choices = Enum.GetNames(typeof(OperatorType)).ToList();

            popupField = new PopupField<string>(choices, 0, selectedName => {
                operatorToUse = (OperatorType)Enum.Parse(typeof(OperatorType), selectedName);
                return selectedName;
            });
        }

        //public void Evaluate()
        //{
        //    IPropertyNode connectedInputNode1 = inputPort1.connections.First(edge => edge.output.node != this).output.node as IPropertyNode;
        //    IPropertyNode connectedInputNode2 = inputPort2.connections.First(edge => edge.output.node != this).output.node as IPropertyNode;

        //    if (connectedInputNode1 is IntegerPropertyNode)
        //    {
        //        outputContainer.Q<Port>("Output").userData = ((IntegerPropertyNode)connectedInputNode1).Value + ((IntegerPropertyNode)connectedInputNode2).Value;
        //    }
        //}
    }
}
