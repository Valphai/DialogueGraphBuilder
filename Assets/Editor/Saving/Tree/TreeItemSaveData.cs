using System.Collections.Generic;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class TreeItemSaveData
    {
        public DialogueTreeItem rootItem;
        public List<TreeItemSaveData> children;

        public TreeItemSaveData(DialogueTreeItem rootItem, List<TreeItemSaveData> children)
        {
            this.rootItem = rootItem;
            this.children = children;
        }
    }
}