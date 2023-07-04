using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class DialogueNode : BaseNode
    {
        public override string Name { get; set; } = "Dialogue Node";
        public string Text { get; set; }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);
            Text = string.Empty;
        }

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new DialogueNodeSaveData() { text = Text, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            DialogueNodeSaveData dialogueSaveData = (DialogueNodeSaveData)saveData;
            Text = dialogueSaveData.text;
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            contentContainer
                .WithMinHeight(UIStyles.MaxHeight)
                .WithMaxWidth(UIStyles.MaxWidth)
                .WithTextField(Text, evt => Text = evt.newValue);
        }
    }
}
