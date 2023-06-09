using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [System.Serializable]
    public class GraphSaveData
    {
        public List<SituationSaveData> situationSaveData = new List<SituationSaveData>();

        public GraphSaveData()
        {
        }
    }
}