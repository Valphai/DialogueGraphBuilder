using Chocolate4.Dialogue.Runtime.Utilities;
using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [Serializable]
    public class SituationSaveData
    {
        public string situationGuid;
        public List<NodeSaveData> nodeData;
        public List<DialogueNodeSaveData> dialogueNodeData;
        public List<OperatorNodeSaveData> operatorNodeData;
        public List<EqualityNodeSaveData> equalityNodeData;
        //public List<GroupSaveData> groupData;

        public SituationSaveData(string situationGuid, List<IDataHolder> dataHolders)
        {
            nodeData = new List<NodeSaveData>();
            dialogueNodeData = new List<DialogueNodeSaveData>();
            operatorNodeData = new List<OperatorNodeSaveData>();
            equalityNodeData = new List<EqualityNodeSaveData>();

            this.situationGuid = situationGuid;
            //this.groupData = groupData;

            if (dataHolders.IsNullOrEmpty())
            {
                return;
            }

            TypeExtensions.DistributeListElementsToFieldsOfImplementingTypes(dataHolders, this);
        }

        public bool TryMergeDataIntoHolder(out List<IDataHolder> dataHolders)
        {
            dataHolders = 
                TypeExtensions.MergeFieldListsIntoOneImplementingType<IDataHolder, SituationSaveData>(this);

            return !dataHolders.IsNullOrEmpty();
        }
    }
}
