using Chocolate4.Dialogue.Runtime.Saving;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Saving
{
    [System.Serializable]
    internal class DataContainer
    {
        [field: SerializeField]
        public GraphSaveData GraphData { get; private set; }
        [field: SerializeField]
        public TreeSaveData TreeData { get; private set; }

        internal void Store(GraphSaveData graphData, TreeSaveData treeData)
        {
            //UpdateGraphData(graphData);
            GraphData = graphData;
            TreeData = treeData;
        }
    }
}
