using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class EqualityNodeSaveData : IDataHolder
    {
        public EqualityType equalityEnum;
        public List<string> constantViewValues;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}
