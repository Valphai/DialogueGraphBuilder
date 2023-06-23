using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class OperatorNodeSaveData : IDataHolder
    {
        public OperatorType operatorEnum;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}
