using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [System.Serializable]
    public class TreeSaveData
    {
        public int selectedIndex = 0;
        public List<TreeItemSaveData> treeItemData;

        public TreeSaveData(List<TreeItemSaveData> treeItemData)
        {
            this.treeItemData = treeItemData;
        }
    }
}