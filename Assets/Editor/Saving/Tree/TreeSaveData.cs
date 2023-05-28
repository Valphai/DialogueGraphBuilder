using Chocolate4.Editor;
using System.Collections.Generic;

namespace Chocolate4.Saving
{
    [System.Serializable]
    public class TreeSaveData : SaveData<TreeItemSaveData>
    {
        public List<TreeItemSaveData> treeItemData;

        public TreeSaveData(List<TreeItemSaveData> treeItemData)
        {
            this.treeItemData = treeItemData;
        }
    }
}