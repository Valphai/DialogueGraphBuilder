using Chocolate4.Saving;
using UnityEngine;

namespace Chocolate4.Editor.Saving
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
