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
            //switch (FloatType)
            //{
            //    case FloatType.Integer:
            //        return new IntegerNode { value = (int)value };

            //}

            IntegerPropertyNode node = new IntegerPropertyNode() { PropertyGuid = Guid, Value = Value };
            //node.FindInputSlot<Vector1MaterialSlot>(Vector1Node.InputSlotXId).value = value;
            return node;
        }

        public override IDialogueProperty Copy() => new IntegerDialogueProperty()
        {
            DisplayName = DisplayName,
            Value = Value
        };
    }
}