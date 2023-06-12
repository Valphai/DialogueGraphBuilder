using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class FloatDialogueProperty : DialogueProperty<float>
    {
        public override PropertyType PropertyType => PropertyType.Float;

        public FloatDialogueProperty()
        {
            DisplayName = "Float";
        }

        public override BaseNode ToConcreteNode()
        {
            //switch (FloatType)
            //{
            //    case FloatType.Integer:
            //        return new IntegerNode { value = (int)value };

            //}

            BaseNode node = new DialogueNode();
            //node.FindInputSlot<Vector1MaterialSlot>(Vector1Node.InputSlotXId).value = value;
            return node;
        }

        public override IDialogueProperty Copy() => new FloatDialogueProperty()
        {
            DisplayName = DisplayName,
            Value = Value
        };
    }
}