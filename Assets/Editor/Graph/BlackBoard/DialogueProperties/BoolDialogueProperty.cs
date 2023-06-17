using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class BoolDialogueProperty : DialogueProperty<bool>
    {
        public override PropertyType PropertyType => PropertyType.Bool;

        public BoolDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
        }

        public override BaseNode ToConcreteNode()
        {
            BoolPropertyNode node = new BoolPropertyNode()
            {
                Name = DisplayName,
                PropertyGuid = Guid,
                Value = Value
            };

            return node;
        }
    }
}