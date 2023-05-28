using Chocolate4.Saving;
using System;
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
            GraphData = graphData;
            TreeData = treeData;
        }
    }
}
