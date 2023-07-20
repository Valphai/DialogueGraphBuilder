using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Entities;
using Chocolate4.Dialogue.Edit.Graph;

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

        internal void Rebuild(DialogueTreeView treeView, DialogueGraphView graphView, DialogueEntitiesView entitiesView)
        {
            treeView.Rebuild(dataContainer.TreeData);
            graphView.Rebuild(dataContainer.GraphData);
            entitiesView.Rebuild(dataContainer.EntitiesData);
        }

        internal void Store(GraphSaveData graphData, TreeSaveData treeData, EntitiesData entitiesData)
        {
            dataContainer.Store(graphData, treeData, entitiesData);
        }
    }
}
