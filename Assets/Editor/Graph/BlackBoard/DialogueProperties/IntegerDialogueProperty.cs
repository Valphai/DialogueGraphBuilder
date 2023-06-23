using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class IntegerDialogueProperty : DialogueProperty<int>
    {
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
                PropertyID = Id,
                Value = Value
            };

            return node;
        }
    }
}