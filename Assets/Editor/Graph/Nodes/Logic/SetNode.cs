using Chocolate4.Dialogue.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Runtime.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class SetNode : BaseNode
    {
        private List<DynamicPort> dynamicPorts = new List<DynamicPort>();

        public ConstantViewDrawer ConstantViewDrawer { get; protected set; }
        public override string Name { get; set; } = "Set Node";

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();

            List<string> savedConstantViewValues = ConstantViewDrawer.Save();
            return new SetNodeSaveData() { constantViewValues = savedConstantViewValues, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            SetNodeSaveData setSaveData = (SetNodeSaveData)saveData;

            ConstantViewDrawer.Load(setSaveData.constantViewValues);
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
            base.DrawTitle();
            titleContainer.WithLogicStyle();
        }

        protected override void DrawInputPort()
        {
            DynamicPort inputPort1 = PortUtilities.DrawDynamicPort(NodeConstants.Input1, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));
            DynamicPort inputPort2 = PortUtilities.DrawDynamicPort(NodeConstants.Input2, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));
            Port inputPort3 = DrawPort(NodeConstants.TransferIn, Direction.Input, Port.Capacity.Single, typeof(ExtraOperationPortType));

            inputContainer.Add(inputPort1);
            inputContainer.Add(inputPort2);
            inputContainer.Add(inputPort3);
        }

        protected override void DrawOutputPort()
        {
            Port outputPort = DrawPort(NodeConstants.TransferOut, Direction.Output, Port.Capacity.Single, typeof(ExtraOperationPortType));
            outputContainer.Add(outputPort);
        }
    }
}