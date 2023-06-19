using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class EqualNode : BaseNode
    {
        private const string Port1 = "Input 1";
        private const string Port2 = "Input 2";

        private PopupField<string> popupField;
        private EqualityType equalityTypeToUse;

        public override NodeTask NodeTask { get; set; } = NodeTask.Logic;
        public override string Name { get; set; } = "Equality Node";

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            if (direction == Direction.Output)
            {
                return node.NodeTask == NodeTask.Dialogue || node.NodeTask == NodeTask.Logic;
            }

            return node.NodeTask == NodeTask.Property;
        }

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
            Port inputPort1 = DrawPort(Port1, Direction.Input, Port.Capacity.Single);
            Port inputPort2 = DrawPort(Port2, Direction.Input, Port.Capacity.Single);

            inputContainer.Add(inputPort1);
            inputContainer.Add(inputPort2);
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
