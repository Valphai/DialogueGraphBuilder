using Chocolate4.Dialogue.Runtime.Saving;
using System;
using System.Collections.Generic;

namespace Chocolate4.Edit.Graph.Utilities
{
    public class SituationCache
    {
        private List<SituationSaveData> situationToData;

        public IReadOnlyCollection<SituationSaveData> SituationToData => situationToData;

        public event Action<string> OnSituationCached;

        public SituationCache(List<SituationSaveData> newSituationSaveData = null)
        {
            situationToData = newSituationSaveData == null
                ? new List<SituationSaveData>()
                : new List<SituationSaveData>(newSituationSaveData);
        }

        public void TryCache(SituationSaveData situationSaveData)
        {
            if (!IsCached(situationSaveData.situationGuid, out SituationSaveData cachedSituationSaveData))
            {
                situationToData.Add(situationSaveData);

                OnSituationCached?.Invoke(situationSaveData.situationGuid);
                return;
            }

            int cachedIndex = situationToData.IndexOf(cachedSituationSaveData);
            situationToData[cachedIndex] = situationSaveData;
        }

        public bool IsCached(string situationGuid, out SituationSaveData cachedSaveData)
        {
            cachedSaveData = situationToData.Find(
                situation => situation.situationGuid == situationGuid
            );

            return cachedSaveData != null;
        }

        public bool TryRemove(SituationSaveData situationSaveData)
        {
            return situationToData.Remove(situationSaveData);
        }
    } 
}