using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Edit.Graph;

namespace Chocolate4.Dialogue.Edit.Saving
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
