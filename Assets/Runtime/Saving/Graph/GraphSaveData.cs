using System.Collections.Generic;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [System.Serializable]
    public class GraphSaveData
    {
        public Vector2 graphViewPosition;
        public Vector2 graphViewZoom;
        public List<SituationSaveData> situationSaveData = new List<SituationSaveData>();
        public BlackboardSaveData blackboardSaveData;
    }
}