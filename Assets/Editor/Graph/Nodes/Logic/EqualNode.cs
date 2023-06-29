using Chocolate4.Dialogue.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NodeConstants = Chocolate4.Runtime.Utilities.NodeConstants;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class EqualNode : BaseNode, IConstantViewDraw
    {
        private PopupField<string> popupField;
        private EqualityType equalityTypeToUse;
        private List<DynamicPort> dynamicPorts = new List<DynamicPort>();

        public ConstantViewDrawer ConstantViewDrawer { get; protected set; }
        public override string Name { get; set; } = "Equality Node";

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            List<string> savedConstantViewValues = ConstantViewDrawer.Save();
            return new EqualityNodeSaveData() 
            { 
                equalityEnum = equalityTypeToUse, 
                nodeSaveData = saveData,
                constantViewValues = savedConstantViewValues
            };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            EqualityNodeSaveData equalitySaveData = (EqualityNodeSaveData)saveData;
            equalityTypeToUse = equalitySaveData.equalityEnum;

            ConstantViewDrawer.Load(equalitySaveData.constantViewValues);
        }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);

            ConstantViewDrawer = new ConstantViewDrawer();

            VisualElement element = this.Q<VisualElement>("node-border");
            element?.WithOverflow();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            dynamicPorts.AddRange(
                inputContainer.Query<DynamicPort>().ToList()
            );

            dynamicPorts.ForEach(port =>
                ConstantViewDrawer.ConstantViews.AddRange(ConstantViewUtilities.CreatePossibleConstantViews(port))
            );

            PortUtilities.SetDynamicPorts(dynamicPorts, typeof(AnyValuePortType));
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
            DynamicPort inputPort1 = PortUtilities.DrawDynamicPort(NodeConstants.Input1, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));
            DynamicPort inputPort2 = PortUtilities.DrawDynamicPort(NodeConstants.Input2, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));

            inputContainer.Add(inputPort1);
            inputContainer.Add(inputPort2);
        }

        protected override void DrawOutputPort()
        {
            Port outputPort = DrawPort(NodeConstants.PropertyOutput, Direction.Output, Port.Capacity.Single, typeof(BoolPortType));
            outputContainer.Add(outputPort);
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
