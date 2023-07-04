using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);
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

            foreach (DialogueChoice choice in choices)
            {
                AddChoicePort(choice);
            }
        }

        private void AddChoicePort(DialogueChoice choice)
        {
            VisualElement container = new VisualElement()
                .WithFlexGrow()
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
    }

    public class DialogueChoice
    {
        public string name;
        public string text;
    }
}
