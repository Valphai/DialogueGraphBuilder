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
        private Port inputPort1;
        private Port inputPort2;

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

        public void UpdatePortTypes(Port connectingPort)
        {
            if (connectingPort.direction == Direction.Output)
            {
                if (!inputPort1.connected && !inputPort2.connected)
                {
                    inputPort1.portType = inputPort2.portType = typeof(AnyValuePortType);
                }
                
                if (NodeUtilities.IsPortConnectionAllowed(connectingPort, inputPort1) 
                    && NodeUtilities.IsPortConnectionAllowed(connectingPort, inputPort2)
                )
                {
                    inputPort1.portType = inputPort2.portType = connectingPort.portType;
                }

                InputPortDataCollection[0].thisPortType = inputPort1.portType.ToString();
                InputPortDataCollection[1].thisPortType = inputPort2.portType.ToString();
            }
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
            inputPort1 = DrawPort(Port1, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));
            inputPort2 = DrawPort(Port2, Direction.Input, Port.Capacity.Single, typeof(AnyValuePortType));

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
