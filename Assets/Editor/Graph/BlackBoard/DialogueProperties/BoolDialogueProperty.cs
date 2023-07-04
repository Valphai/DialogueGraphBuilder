using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class BoolDialogueProperty : ValueDialogueProperty<bool>
    {
        private BoolConstantView boolConstantView;

        public override PropertyType PropertyType { get; protected set; } = PropertyType.Bool;

        public BoolDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
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