using Chocolate4.Saving;
using System;
using UnityEngine;

namespace Chocolate4
{
    public class DialogueEditorAsset : ScriptableObject
    {
        public GraphSaveData graphSaveData;
        public TreeSaveData treeSaveData;

        public void LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            JsonSaveData parsedJson = JsonUtility.FromJson<JsonSaveData>(json);
            parsedJson.ToAsset(this);
        }

        public string ToJson()
        {
            var fileJson = new JsonSaveData(graphSaveData, treeSaveData);
            return JsonUtility.ToJson(fileJson, true);
        }
    }

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