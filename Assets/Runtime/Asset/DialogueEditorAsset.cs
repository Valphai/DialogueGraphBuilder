using Chocolate4.Dialogue.Runtime.Saving;
using System;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Asset
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
}