using System.Collections.Generic;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class GraphSaveData
    {
        public List<SituationSaveData> situationSaveData = new List<SituationSaveData>();

        public GraphSaveData()
        {
        }

        public GraphSaveData(string situationGuid, List<NodeSaveData> nodeData)
        {
            situationSaveData.Add(new SituationSaveData(situationGuid, nodeData));
        }
    }
}