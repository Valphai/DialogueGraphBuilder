using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [System.Serializable]
    public class EntitiesData
    {
        public List<DialogueEntity> cachedEntities = new List<DialogueEntity>();
    }
}