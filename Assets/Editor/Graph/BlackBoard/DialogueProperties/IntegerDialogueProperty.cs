using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class IntegerDialogueProperty : DialogueProperty<int>
    {
        public override PropertyType PropertyType => PropertyType.Integer;

        public IntegerDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
        }

        public override BaseNode ToConcreteNode()
        {
            IntegerPropertyNode node = new IntegerPropertyNode()
            {
                Name = DisplayName,
                PropertyGuid = Guid,
                Value = Value
            };

            return node;
        }
    }
}