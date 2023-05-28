using Chocolate4.Editor.Saving;
using Chocolate4.Saving;
using Chocolate4.Tree;

namespace Chocolate4.Editor
{
    [System.Serializable]
    internal class DataRebuilder
    {
        public DataContainer dataContainer;

        public DataRebuilder()
        {
            dataContainer = new DataContainer();
        }

        internal void Rebuild(DialogueTreeView treeView, DialogueGraphView graphView)
        {
            treeView.Rebuild(dataContainer.TreeData);
            graphView.Rebuild(dataContainer.GraphData);
        }

        internal void Store(GraphSaveData graphData, TreeSaveData treeData)
        {
            dataContainer.Store(graphData, treeData);
        }
    }
}
