using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class EqualityNodeSaveData : IDataHolder
    {
        public EqualityType equalityEnum;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}
