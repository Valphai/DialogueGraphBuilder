using Chocolate4.Editor.Graph.Utilities;
using Chocolate4.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace Chocolate4.Editor.Saving
{
    public class StructureSaver
    {
        private struct NodeDepthContainer
        {
            public readonly int depth;
            public readonly IEnumerable<int> children;

            public NodeDepthContainer(int depth, IEnumerable<int> children)
            {
                this.depth = depth;
                this.children = children;
            }
        }

        public static GraphSaveData SaveGraph(List<SituationSaveData> situationToData)
        {
            return new GraphSaveData()
            {
                situationSaveData = situationToData
            };
        }

        public static SituationSaveData SaveSituation(
            string situationGuid, UQueryState<GraphElement> graphElements
        )
        {
            List<NodeSaveData> nodeSaveDatas = new List<NodeSaveData>();

            graphElements.ForEach(element => {
                if (element is BaseNode node)
                {
                    nodeSaveDatas.Add(SaveNode(node));
                }
            });

            return new SituationSaveData(situationGuid, nodeSaveDatas);

            NodeSaveData SaveNode(BaseNode node)
            {
                Port inputPort = node.inputContainer.Q<Port>();
                var connectionsMap = NodeUtilities.GetConnections(inputPort);

                if (!connectionsMap.IsNullOrEmpty())
                {
                    connectionsMap.ForEach(parent => {
                        if (!node.InputIDs.Contains(parent.ID))
                        {
                            node.InputIDs.Add(parent.ID);
                        }
                    });
                }

                return new NodeSaveData(node);
            }
        }

        public static TreeSaveData SaveTree(TreeView treeView)
        {
            //IEnumerable<int> rootIds = treeView.GetRootIds();

            //List<TreeItemSaveData> itemsSaveData = new List<TreeItemSaveData>();
            //TreeSaveData treeSaveData = new TreeSaveData(itemsSaveData);

            Dictionary<int, IEnumerable<int>> parentToChildrenID = new Dictionary<int, IEnumerable<int>>();

            // go deepest and push children onto stuck

            int treeCount = treeView.GetTreeCount();

            for (int i = 0; i < treeCount; i++)
            {
                int nextId = treeView.GetIdForIndex(i);
                IEnumerable<int> childIds = treeView.viewController.GetChildrenIds(nextId);

                parentToChildrenID.Add(nextId, childIds);
            }

            List<TreeItemSaveData> resultData = new List<TreeItemSaveData>();
            TreeSaveData result = new TreeSaveData(resultData);

            var deepestChildIds = parentToChildrenID.ToList();
            deepestChildIds.Sort((a, b) => treeView.GetDepthOfItemById(a.Key).CompareTo(treeView.GetDepthOfItemById(b.Key)));

            //deepestChildIds.Reverse();

            foreach (var deepestChildId in deepestChildIds)
            {
                int parentId = deepestChildId.Key;
                //IEnumerable<int> childrenIds = deepestChildId.Value;
                //DialogueTreeItem parent = treeView.GetItemDataForId<DialogueTreeItem>(parentId);
                //List<TreeItemSaveData> childrenSaveData = new List<TreeItemSaveData>();
                //TreeItemSaveData nodeSaveData = new TreeItemSaveData(parent, childrenSaveData);

                List<TreeItemSaveData> childrenSaveData = new List<TreeItemSaveData>();
                TreeItemSaveData nodeSaveData = null;

                while (parentId != -1)
                {
                    /// save child
                    DialogueTreeItem parent = treeView.GetItemDataForId<DialogueTreeItem>(parentId);


                    if (nodeSaveData != null)
                    {
                        childrenSaveData.Add(nodeSaveData);
                    }

                    nodeSaveData = new TreeItemSaveData(parent, childrenSaveData.ToList());
                    ///

                    TreeItemSaveData alreadySavedData = resultData.Find(saveData => saveData.rootItem == parent);
                    if (alreadySavedData != null)
                    {
                        alreadySavedData.children.AddRange(childrenSaveData);
                    }
                    else
                    {
                        resultData.Add(nodeSaveData);
                    }

                    parentId = treeView.viewController.GetParentId(parentId);
                }
            }

            return result;
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
