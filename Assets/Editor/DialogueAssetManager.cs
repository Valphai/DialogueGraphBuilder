using Chocolate4.Editor;
using Chocolate4.Saving;
using Chocolate4.Tree;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chocolate4
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