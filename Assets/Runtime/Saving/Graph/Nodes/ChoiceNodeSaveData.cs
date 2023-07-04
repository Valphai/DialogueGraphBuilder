using Chocolate4.Dialogue.Runtime.Nodes;
using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class ChoiceNodeSaveData : IDataHolder
    {
        public List<DialogueChoice> choices;
        public NodeSaveData nodeSaveData;

        public NodeSaveData NodeData => nodeSaveData;
    }
}