using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class ChoiceNode : BaseNode
    {
        private List<DialogueChoice> choices = new List<DialogueChoice>();
        private VisualElement buttonedPortsContainer;
        private VisualElement extraContentContainer;

        private int ChoicesCount => choices.Count;

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
                    DialogueChoice choice = new DialogueChoice() { name = $"Choice {ChoicesCount + 1}" };
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
            VisualElement container = new VisualElement()
                .WithHorizontalGrow();
            
            Port outputPort = DrawPort(choice.name, Direction.Output, Port.Capacity.Single, typeof(TransitionPortType));

            Foldout extraContentFoldout = extraContentContainer.WithFoldout(choice.name);
            TextField extraContentTextField = extraContentContainer
                    .WithTextField(choice.text, evt => choice.text = evt.newValue);
            
            extraContentFoldout.Add(extraContentTextField);

            container
                .WithButton("-")
                .WithOnClick(() => RemoveChoicePort(choice, container, extraContentFoldout));

            container.Add(outputPort);
            buttonedPortsContainer.Add(container);

            CreatePortData(outputContainer, OutputPortDataCollection);
        }

        private void RemoveChoicePort(
            DialogueChoice choice, VisualElement container, 
            VisualElement extraContentFoldout
            )
        {
            buttonedPortsContainer.Remove(container);
            extraContentContainer.Remove(extraContentFoldout);
            choices.Remove(choice);

            CreatePortData(outputContainer, OutputPortDataCollection);
        }
    }
}
