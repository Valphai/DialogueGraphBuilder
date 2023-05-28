using System.Collections.Generic;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class SituationSaveData
    {
        public string situationGuid;
        public List<NodeSaveData> nodeData;
        public List<GroupSaveData> groupData;

        public SituationSaveData(string situationGuid, List<NodeSaveData> nodeData)
        {
            this.situationGuid = situationGuid;
            this.nodeData = nodeData;
        }
    }
}
