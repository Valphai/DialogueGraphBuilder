using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class IntegerDialogueProperty : ValueDialogueProperty<int>
    {
        private IntegerConstantView integerConstantView;

        public override PropertyType PropertyType { get; protected set; } = PropertyType.Integer;

        public IntegerDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
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