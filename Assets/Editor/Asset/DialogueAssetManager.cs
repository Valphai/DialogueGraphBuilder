using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Edit.Graph;
using System.IO;
using UnityEditor;
using UnityEngine;
using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Asset;

namespace Chocolate4.Dialogue.Edit.Asset
{
    [System.Serializable]
    public class DialogueAssetManager
    {
        [SerializeField] 
        public DialogueEditorAsset ImportedAsset { get; private set; }
        [SerializeField] 
        private int instanceId;

        private DataRebuilder dataRebuilder;

        private string Path => AssetDatabase.GetAssetPath(instanceId);

        public DialogueAssetManager(DialogueEditorAsset importedAsset, int instanceId)
        {
            dataRebuilder = new DataRebuilder();

            this.ImportedAsset = importedAsset;
            this.instanceId = instanceId;
            Store(importedAsset.graphSaveData, importedAsset.treeSaveData);
        }

        internal void Rebuild(DialogueTreeView treeView, DialogueGraphView graphView)
        {
            dataRebuilder.Rebuild(treeView, graphView);
        }

        internal void Store(GraphSaveData graphData, TreeSaveData treeData)
        {
            dataRebuilder.Store(graphData, treeData);
        }

        internal void Save(GraphSaveData graphData, TreeSaveData treeData)
        {
            Debug.Assert(ImportedAsset != null);

            Store(graphData, treeData);

            ImportedAsset.graphSaveData = dataRebuilder.dataContainer.GraphData;
            ImportedAsset.treeSaveData = dataRebuilder.dataContainer.TreeData;

            string assetJson = ImportedAsset.ToJson();
            string existingJson = File.ReadAllText(Path);

            if (assetJson != existingJson)
            {
                File.WriteAllText(Path, assetJson);
                AssetDatabase.ImportAsset(Path);
            }
        }
    }
}