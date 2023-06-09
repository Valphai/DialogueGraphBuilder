using System.Collections.Generic;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class TreeItemSaveData
    {
        public int depth;
        public DialogueTreeItem rootItem;
        public List<string> childrenGuids;

        public TreeItemSaveData(DialogueTreeItem rootItem, List<string> childrenGuids, int depth)
        {
            this.rootItem = rootItem;
            this.childrenGuids = childrenGuids;
            this.depth = depth;
        }
    }
}