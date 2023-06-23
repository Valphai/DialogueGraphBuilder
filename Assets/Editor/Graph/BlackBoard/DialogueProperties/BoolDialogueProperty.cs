using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class BoolDialogueProperty : DialogueProperty<bool>
    {
        private BoolConstantView boolConstantView;

        public override PropertyType PropertyType { get; protected set; } = PropertyType.Bool;

        public BoolDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
        }

        public override BaseNode ToConcreteNode()
        {
            BoolPropertyNode node = new BoolPropertyNode()
            {
                Name = DisplayName,
                PropertyId = Id,
                Value = Value
            };

            return node;
        }

        public override IConstantViewControlCreator ToConstantView()
        {
            boolConstantView = new BoolConstantView((value) => Value = value);
            return boolConstantView;
        }

        public override void UpdateConstantView()
        {
            boolConstantView.UpdateControl(Value);
        }
    }
}