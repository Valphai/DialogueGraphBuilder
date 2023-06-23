using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class IntegerDialogueProperty : DialogueProperty<int>
    {
        private IntegerConstantView integerConstantView;

        public override PropertyType PropertyType { get; protected set; } = PropertyType.Integer;

        public IntegerDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
        }

        public override BaseNode ToConcreteNode()
        {
            IntegerPropertyNode node = new IntegerPropertyNode()
            {
                Name = DisplayName,
                PropertyId = Id,
                Value = Value
            };

            return node;
        }

        public override IConstantViewControlCreator ToConstantView()
        {
            integerConstantView = new IntegerConstantView((value) => Value = value);
            return integerConstantView;
        }

        public override void UpdateConstantView()
        {
            integerConstantView.UpdateControl(Value);
        }
    }
}