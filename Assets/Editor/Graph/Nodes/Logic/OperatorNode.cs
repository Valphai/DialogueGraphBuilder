using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class OperatorNode : BaseNode//, ILogicEvaluate
    {
        private const string Port1 = "Input 1";
        private const string Port2 = "Input 2";

        private Port inputPort1;
        private Port inputPort2;
        private PopupField<string> popupField;
        private OperatorType operatorToUse;

        public override string Name { get; set; } = "Operator";
        public override NodeTask NodeTask { get; set; } = NodeTask.Logic;

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            if (node.NodeTask != NodeTask.Property)
            {
                return false;
            }

            PropertyType nodePropertyType = (node as IPropertyNode).PropertyType;
            IPropertyNode connectedInputNode1 = null;
            IPropertyNode connectedInputNode2 = null;

            if (!inputPort1.connections.IsNullOrEmpty())
            {
                connectedInputNode1 = inputPort1.connections.First(edge => edge.output.node != this).output.node as IPropertyNode; 
            }
            if (!inputPort2.connections.IsNullOrEmpty())
            {
                connectedInputNode2 = inputPort2.connections.First(edge => edge.output.node != this).output.node as IPropertyNode;
            }

            if (connectedInputNode1 == null && connectedInputNode2 == null)
            {
                return true;
            }

            if (IsSamePropertyType(nodePropertyType, connectedInputNode1)
                || IsSamePropertyType(nodePropertyType, connectedInputNode2)
            )
            {
                return true;
            }

            return false;

            bool IsSamePropertyType(PropertyType nodePropertyType, IPropertyNode alreadyConnectedNode)
            {
                if (alreadyConnectedNode == null)
                {
                    return false;
                }

                return nodePropertyType == alreadyConnectedNode.PropertyType;
            }
        }

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

        protected override void DrawInputPort()
        {
            inputPort1 = DrawPort(Port1, Direction.Input, Port.Capacity.Single);
            inputPort2 = DrawPort(Port2, Direction.Input, Port.Capacity.Single);

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
