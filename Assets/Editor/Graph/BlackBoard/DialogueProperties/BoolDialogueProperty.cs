using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class BoolDialogueProperty : DialogueProperty<bool>
    {
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
                PropertyID = Id,
                Value = Value
            };

            return node;
        }
    }
}