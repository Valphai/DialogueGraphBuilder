using Chocolate4.Dialogue.Runtime.Saving;
using System;

namespace Chocolate4.Dialogue.Runtime.Asset
{
    [Serializable]
    public struct JsonSaveData
    {
        public string collectionName;
        public GraphSaveData graphSaveData;
        public TreeSaveData treeSaveData;

        public JsonSaveData(string collectionName, GraphSaveData graphSaveData, TreeSaveData treeSaveData)
        {
            this.collectionName = collectionName;
            this.graphSaveData = graphSaveData;
            this.treeSaveData = treeSaveData;
        }

        public void ToAsset(DialogueEditorAsset dialogueEditorAsset)
        {
            dialogueEditorAsset.fileName = collectionName;
            dialogueEditorAsset.treeSaveData = treeSaveData;
            dialogueEditorAsset.graphSaveData = graphSaveData;
        }
    }
}