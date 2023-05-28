using Chocolate4.Editor;
using Chocolate4.Saving;
using Chocolate4.Tree;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4
{
    [System.Serializable]
    public class DialogueAssetManager
    {
        [SerializeField] 
        public DialogueEditorAsset ImportedAsset { get; private set; }
        [SerializeField] 
        private int instanceId;
        [SerializeField]
        private GraphSaveData graphData;
        [SerializeField]
        private TreeSaveData treeData;

        private string Path => AssetDatabase.GetAssetPath(instanceId);

        public DialogueAssetManager(DialogueEditorAsset importedAsset, int instanceId)
        {
            this.ImportedAsset = importedAsset;
            this.instanceId = instanceId;
            Store(importedAsset.graphSaveData, importedAsset.treeSaveData);
        }

        internal void Rebuild(DialogueTreeView treeView, DialogueGraphView graphView)
        {
            treeView.Rebuild(treeData);
            graphView.LoadGraph(graphData);
        }

        internal void Store(GraphSaveData graphData, TreeSaveData treeData)
        {
            this.treeData = treeData;
            this.graphData = graphData;
        }

        public void Store(SaveData<TreeItemSaveData> data)
        {

        }

        internal void Save(GraphSaveData graphData, TreeSaveData treeData)
        {
            Debug.Assert(ImportedAsset != null);

            Store(graphData, treeData);

            ImportedAsset.graphSaveData = graphData;
            ImportedAsset.treeSaveData = treeData;

            string assetJson = ImportedAsset.ToJson();
            string existingJson = File.ReadAllText(Path);

            if (assetJson != existingJson)
            {
                File.WriteAllText(Path, assetJson);
                AssetDatabase.ImportAsset(Path);
            }

            //m_IsDirty = false;
        }

        public static SituationSaveData SaveSituation(
            string situationGuid, Dictionary<BaseNode, List<BaseNode>> nodeToOtherNodes
        )
        {
            List<NodeSaveData> nodeSaveData = new List<NodeSaveData>();
            foreach (var pair in nodeToOtherNodes)
            {
                nodeSaveData.Add(new NodeSaveData(pair.Key, pair.Value));
            }

            return new SituationSaveData(situationGuid, nodeSaveData);
        }

        public static TreeSaveData SaveTree(TreeView treeView)
        {
            int treeItemsCount = treeView.GetTreeCount();
            IEnumerable<int> rootIds = treeView.GetRootIds();

            List<TreeItemSaveData> itemSaveData = new List<TreeItemSaveData>();
            TreeSaveData treeSaveData = new TreeSaveData(itemSaveData);

            for (int i = 0; i < treeItemsCount; i++)
            {
                int rootId = treeView.GetIdForIndex(i);
                if (!rootIds.Any(root => root == rootId))
                {
                    continue;
                }

                DialogueTreeItem rootItem = treeView.GetItemDataForId<DialogueTreeItem>(rootId);
                itemSaveData.Add(
                    GetChildren(rootItem, i, treeView)
                    //new TreeItemSaveData(rootItem, GetChildren(rootItem, rootId, treeView))
                );
            }


            //for (int i = 0; i < treeItemsCount; i++)
            //{
            //    if (!rootIds.Any(rootId => rootId == treeView.GetIdForIndex(i)))
            //    {
            //        continue;
            //    }

            //    SampleTreeItem treeItem = treeView.GetItemDataForIndex<SampleTreeItem>(i);
            //    List<SampleTreeItem> children = new List<SampleTreeItem>();

            //    foreach (int childId in treeView.GetChildrenIdsForIndex(i))
            //    {
            //        SampleTreeItem treeItemChild = treeView.GetItemDataForId<SampleTreeItem>(childId);
            //        children.Add(treeItemChild);
            //    }

            //    itemSaveData.Add(new TreeItemSaveData(treeItem, children));
            //}

            return treeSaveData;
        }

        private static TreeItemSaveData GetChildren(DialogueTreeItem parent, int parentIndex, TreeView treeView)
        {
            IEnumerable<int> childIds = treeView.GetChildrenIdsForIndex(parentIndex);

            var children = new List<TreeItemSaveData>();
            for (int j = 0; j < childIds.Count(); j++)
            {
                int childId = childIds.ElementAt(j);

                DialogueTreeItem child = treeView.GetItemDataForId<DialogueTreeItem>(childId);
                TreeItemSaveData childSaveData = GetChildren(child, parentIndex + j, treeView);
                children.Add(childSaveData);
            }

            return new TreeItemSaveData(parent, children);
        }
    }
}