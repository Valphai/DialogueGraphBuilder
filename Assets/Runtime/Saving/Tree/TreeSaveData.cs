using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [System.Serializable]
    public class TreeSaveData
    {
        public int selectedIndex = 0;
        public List<TreeItemSaveData> treeItemData;

        public TreeSaveData()
        {
            
        }

        public TreeSaveData(TreeSaveData treeData)
        {
            selectedIndex = treeData.selectedIndex;
            treeItemData = treeData.treeItemData.ToList();
        }
    }
}