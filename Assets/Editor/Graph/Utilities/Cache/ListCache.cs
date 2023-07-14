using System.Collections.Generic;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities
{
    public abstract class ListCache<T> where T : IHaveId
    {
        protected List<T> listCache;

        public IReadOnlyCollection<T> Cache => listCache;

        protected abstract T GetData(string id);

        public virtual bool IsCached(string id, out T cachedSaveData)
        {
            cachedSaveData = GetData(id);
            return cachedSaveData != null;
        }

        public virtual bool TryCache(T data)
        {
            if (!IsCached(data.Id, out T cachedSituationSaveData))
            {
                listCache.Add(data);
                return true;
            }

            int cachedIndex = listCache.IndexOf(cachedSituationSaveData);
            listCache[cachedIndex] = data;
            return false;
        }

        public virtual bool TryRemove(T situationSaveData)
        {
            return listCache.Remove(situationSaveData);
        }
    }
}