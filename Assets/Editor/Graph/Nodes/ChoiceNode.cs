using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Graph.Utilities;
using Chocolate4.Runtime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ChoiceNode : BaseNode
    {
        private List<DialogueChoice> choices = new List<DialogueChoice>();
        private VisualElement buttonedPortsContainer;
        private VisualElement extraContentContainer;

        public override string Name { get; set; } = "Choice Node";

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new ChoiceNodeSaveData() { choices = choices.ToList(), nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            ChoiceNodeSaveData choicesSaveData = (ChoiceNodeSaveData)saveData;
            choices = choicesSaveData.choices.ToList();

            foreach (DialogueChoice choice in choices)
            {
                AddChoicePort(choice);
            }
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            contentContainer
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithBackgroundColor(UIStyles.DefaultDarkColor);

            extraContentContainer = contentContainer;
        }

        protected override void DrawOutputPort()
        {
            outputContainer.WithFlexGrow();
            VisualElement addButtonContainer = new VisualElement();
            addButtonContainer
                .WithButton("+")
                .WithOnClick(() => {
                    string[] existingNames = choices.Select(choice => choice.name).ToArray();
                    string uniqueName = ObjectNames.GetUniqueName(existingNames, NodeConstants.ChoiceOut);

                    DialogueChoice choice = new DialogueChoice() { name = uniqueName };
                    AddChoicePort(choice);
                    choices.Add(choice);
                }
            );

            buttonedPortsContainer = new VisualElement().WithFlexGrow();

            outputContainer.Add(addButtonContainer);
            outputContainer.Add(buttonedPortsContainer);
        }

        private void AddChoicePort(DialogueChoice choice)
        {
            int childCount = extraContentContainer.childCount;
            int choiceIndex = Math.Min(choice.ChoiceIndex, childCount);

            VisualElement container = new VisualElement()
                .WithHorizontalGrow();

            Port outputPort = DrawPort(choice.name, Direction.Output, Port.Capacity.Single, typeof(TransitionPortType));
            outputPort.WithFlexGrow();

            Foldout extraContentFoldout = new Foldout() { text = choice.name };
            TextField extraContentTextField = extraContentFoldout
                .WithNodeTextField(choice.text, evt => choice.text = evt.newValue);

            extraContentContainer.Insert(choiceIndex, extraContentFoldout);

            container
                .WithButton("-")
                .WithOnClick(() => RemoveChoicePort(choice, container, extraContentFoldout));

            container.Add(outputPort);
            buttonedPortsContainer.Insert(choiceIndex, container);

            AddNewPortData(outputPort, OutputPortDataCollection);
        }

        private void RemoveChoicePort(
            DialogueChoice choice, VisualElement container, 
            VisualElement extraContentFoldout
        )
        {
            buttonedPortsContainer.Remove(container);
            extraContentContainer.Remove(extraContentFoldout);
            choices.Remove(choice);
        }

        private void AddNewPortData(Port port, List<PortData> dataCollection)
        {
            PortData portData = dataCollection.Find(data => data.thisPortType.Equals(port.portName));
            if (portData != null)
            {
                return;
            }

            portData = new PortData()
            {
                thisPortName = port.portName,
                thisPortType = port.portType.ToString()
            };

            dataCollection.Add(portData);
        }
    }
}
