using Chocolate4.Dialogue.Runtime.Saving;
using System;

namespace Chocolate4.Dialogue.Runtime.Asset
{
    [Serializable]
    public struct JsonSaveData
    {
        public GraphSaveData graphSaveData;
        public TreeSaveData treeSaveData;

        public JsonSaveData(GraphSaveData graphSaveData, TreeSaveData treeSaveData)
        {
            this.graphSaveData = graphSaveData;
            this.treeSaveData = treeSaveData;
        }

        public void ToAsset(DialogueEditorAsset dialogueEditorAsset)
        {
            dialogueEditorAsset.treeSaveData = treeSaveData;
            dialogueEditorAsset.graphSaveData = graphSaveData;
        }
    }
}