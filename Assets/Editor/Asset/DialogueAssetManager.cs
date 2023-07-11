using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Edit.Graph;
using System.IO;
using UnityEditor;
using UnityEngine;
using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Edit.CodeGeneration;
using System.Linq;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Entities;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Edit.Entities.Utilities;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Edit.Asset
{
    [System.Serializable]
    public class DialogueAssetManager
    {
        [SerializeField] 
        private int instanceId;
        [SerializeField]
        private EntitiesHolder entitiesDatabase;

        private DataRebuilder dataRebuilder;

        [field:SerializeField] 
        public DialogueEditorAsset ImportedAsset { get; private set; }

        private string Path => AssetDatabase.GetAssetPath(instanceId);

        public DialogueAssetManager(
            DialogueEditorAsset importedAsset, int instanceId, 
            EntitiesHolder entitiesDatabase
        )
        {
            dataRebuilder = new DataRebuilder();

            this.entitiesDatabase = entitiesDatabase;
            this.instanceId = instanceId;
            ImportedAsset = importedAsset;

            EntitiesData entitiesData = new EntitiesData() 
            {
                cachedEntities = (List<DialogueEntity>)entitiesDatabase.DialogueEntities 
            };

            Store(importedAsset.graphSaveData, importedAsset.treeSaveData, entitiesData);
        }

        internal void Rebuild(DialogueTreeView treeView, DialogueGraphView graphView, DialogueEntitiesView entitiesView)
        {
            entitiesDatabase.Reload();
            dataRebuilder.Rebuild(treeView, graphView, entitiesView);
        }

        internal void Store(GraphSaveData graphData, TreeSaveData treeData, EntitiesData entitiesData)
        {
            dataRebuilder.Store(graphData, treeData, entitiesData);
        }

        internal void Save(GraphSaveData graphData, TreeSaveData treeData, EntitiesData entitiesData)
        {
            Debug.Assert(ImportedAsset != null);

            Store(graphData, treeData, entitiesData);

            string oldFileName = ImportedAsset.fileName;

            ImportedAsset.fileName = System.IO.Path.GetFileNameWithoutExtension(Path);
            ImportedAsset.graphSaveData = dataRebuilder.dataContainer.GraphData;
            ImportedAsset.treeSaveData = dataRebuilder.dataContainer.TreeData;

            SaveEntities(entitiesData);

            TrySaveAssetToFile();

            DialogueMasterCollectionGenerator.TryRegenerate(
                ImportedAsset.fileName, oldFileName, ImportedAsset.graphSaveData.blackboardSaveData
            );
        }

        private void SaveEntities(EntitiesData entitiesData)
        {
            string path =
                FilePathConstants.GetPathRelativeTo(FilePathConstants.Assets, FilePathConstants.dialogueEntitiesPath);

            DialogueEntity[] existingEntities = EntitiesUtilities.GetAllEntities();
            int[] existingIds = existingEntities.Select(entity => entity.GetInstanceID()).ToArray();

            List<int> cachedIds = new List<int>();
            foreach (DialogueEntity entity in entitiesData.cachedEntities)
            {
                int instanceId = entity.GetInstanceID();
                cachedIds.Add(instanceId);

                ScriptableObjectUtilities.CreateAssetAtPath(entity,
                    path, EntitiesUtilities.GetEntityName(entity)
                );
            }

            for (int i = 0; i < existingIds.Length; i++)
            {
                int instanceId = existingIds[i];
                if (!cachedIds.Contains(instanceId))
                {
                    ScriptableObjectUtilities.RemoveAssetAtPath(instanceId);
                }
            }

            entitiesDatabase.Reload();
            AssetDatabase.Refresh();
        }

        private void TrySaveAssetToFile()
        {
            string assetJson = ImportedAsset.ToJson();
            if (FilePathConstants.FileIsDuplicate(Path, assetJson))
            {
                return;
            }

            File.WriteAllText(Path, assetJson);
            AssetDatabase.ImportAsset(Path);
        }
    }
}