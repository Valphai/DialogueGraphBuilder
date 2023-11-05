using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class DialogueNodeSaveData : IDataHolder
    {
        public string text;
        public string speakerIdentifier;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}