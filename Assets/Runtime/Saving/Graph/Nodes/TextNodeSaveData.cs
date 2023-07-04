using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class TextNodeSaveData : IDataHolder
    {
        public string text;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}