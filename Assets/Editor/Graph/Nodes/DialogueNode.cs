using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class DialogueNode : BaseNode, ITextHolder
    {
        private TextField textField;

        public override string Name { get; set; } = "Dialogue Node";
        public string Text { get; set; }

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new TextNodeSaveData() { text = Text, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            TextNodeSaveData textSaveData = (TextNodeSaveData)saveData;
            Text = textSaveData.text;

            textField.value = Text;
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            textField = contentContainer
                .WithMinHeight(UIStyles.MaxHeight)
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithNodeTextField(Text, evt => Text = evt.newValue);
        }
    }
}
