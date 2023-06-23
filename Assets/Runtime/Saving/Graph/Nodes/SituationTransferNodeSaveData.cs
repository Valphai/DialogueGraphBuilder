using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class SituationTransferNodeSaveData : IDataHolder
    {
        public string nextSituationId;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}
