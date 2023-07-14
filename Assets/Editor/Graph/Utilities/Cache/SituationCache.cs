using Chocolate4.Dialogue.Runtime.Saving;
using System;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public class SituationCache : ListCache<SituationSaveData>
    {
        public event Action<string> OnSituationCached;

        public SituationCache(List<SituationSaveData> newSituationSaveData = null)
        {
            listCache = newSituationSaveData == null
                ? new List<SituationSaveData>()
                : new List<SituationSaveData>(newSituationSaveData);
        }

        public override bool TryCache(SituationSaveData situationSaveData)
        {
            if (base.TryCache(situationSaveData))
            {
                OnSituationCached?.Invoke(situationSaveData.situationId);
                return true;
            }

            return false;
        }

        protected override SituationSaveData GetData(string id)
        {
            return listCache.Find(
                situation => situation.situationId == id
            );
        }
    }
}