using Chocolate4.Editor;
using Chocolate4.Editor.Saving;
using Chocolate4.Saving;
using Chocolate4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4
{
    public class DialogueGraphView : GraphView, IRebuildable<GraphSaveData>
    {
        public const string DefaultGroupName = "Dialogue Group";

        private string activeSituationGuid = string.Empty;

        internal SituationCache SituationCache { get; private set; }

        public void Initialize()
        {
            SituationCache = new SituationCache(null);

            AddManipulators();
            AddGridBackground();

            AddStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port => {
                
                if (startPort.node == port.node) return;

                if (startPort.direction == port.direction) return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public GraphSaveData Save()
        {
            CacheActiveSituation();
            return StructureSaver.SaveGraph(SituationCache.SituationToData.ToList());
        }

        public void Rebuild(GraphSaveData graphSaveData)
        {
            if (graphSaveData.situationSaveData.IsNullOrEmpty())
            {
                return;
            }

            SituationCache = new SituationCache(graphSaveData.situationSaveData);

            SituationSaveData situationData = graphSaveData.situationSaveData.Find(
                data => data.situationGuid.Equals(activeSituationGuid)
            );

            if (situationData == null)
            {
                return;
            }

            RebuildGraph(situationData);
        }

        internal void DialogueTreeView_OnSituationSelected(string newSituationGuid)
        {
            if (!activeSituationGuid.Equals(string.Empty))
            {
                CacheActiveSituation(); 
            }

            activeSituationGuid = newSituationGuid;

            if (SituationCache.IsCached(newSituationGuid, out SituationSaveData situationSaveData))
            {
                RebuildGraph(situationSaveData);
            }
        }

        internal void DialogueTreeView_OnTreeItemRemoved(string treeItemGuid)
        {
            if (!SituationCache.IsCached(treeItemGuid, out SituationSaveData cachedSituationSaveData))
            {
                return;
            }

            if (!SituationCache.TryRemove(cachedSituationSaveData))
            {
                Debug.LogError($"Situation {cachedSituationSaveData.situationGuid} could not be removed.");
                return;
            }
        }

        private void CacheActiveSituation()
        {
            SituationSaveData situationSaveData = 
                StructureSaver.SaveSituation(activeSituationGuid, graphElements);

            SituationCache.TryCache(situationSaveData);
        }

        private void RebuildGraph(SituationSaveData situationData)
        {
            DeleteElements(graphElements);

            if (situationData.nodeData == null)
            {
                return;
            }

            List<BaseNode> nodes = new List<BaseNode>();
            IEnumerable<Type> nodeTypes = TypeExtensions.GetTypes<BaseNode>();
            foreach (NodeSaveData nodeData in situationData.nodeData)
            {
                Type matchedType = nodeTypes.First(type => type.ToString().Equals(nodeData.nodeType));
                BaseNode node = CreateNode(nodeData.position, matchedType);
                node.Load(nodeData);
                nodes.Add(node);
            }

            RebuildConnections(nodes);
        }

        private void RebuildConnections(List<BaseNode> nodes)
        {
            foreach (BaseNode node in nodes)
            {
                for (int i = 0; i < node.InputIDs.Count; i++)
                {
                    string parentID = node.InputIDs[i];
                    IEnumerable<BaseNode> connections =
                        nodes.Where(parentNode => parentNode.ID == parentID && parentNode.ID != node.ID);

                    Port inputPort = node.inputContainer.Q<Port>();
                    ConnectPorts(inputPort, connections);
                }
            }
        }

        private void ConnectPorts(Port port, IEnumerable<BaseNode> connections)
        {
            foreach (BaseNode otherNode in connections)
            {
                Port otherPort = otherNode.outputContainer.Q<Port>();

                Edge edge = otherPort.ConnectTo(port);

                AddElement(edge);

                otherNode.RefreshPorts();
            }
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            VisualElementExtensions.AddManipulator(this, new ContentDragger());
            VisualElementExtensions.AddManipulator(this, new SelectionDragger());
            VisualElementExtensions.AddManipulator(this, new RectangleSelector());
            VisualElementExtensions.AddManipulator(this, VisualElementBuilder.CreateContextualMenuManipulator($"Add Group",
                actionEvent => CreateGroup(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))
            ));

            IEnumerable<Type> nodeTypes = TypeExtensions.GetTypes<BaseNode>();

            string contextElementTitle;
            foreach (Type nodeType in nodeTypes)
            {
                contextElementTitle = nodeType.Name;
                VisualElementExtensions.AddManipulator(this, VisualElementBuilder.CreateContextualMenuManipulator($"Add {contextElementTitle}", 
                    actionEvent => CreateNode(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), nodeType)
                ));
            }
        }

        private BaseNode CreateNode(Vector2 startingPosition, Type nodeType)
        {
            BaseNode node = (BaseNode)Activator.CreateInstance(nodeType);

            node.Initialize(startingPosition);
            node.Draw();

            AddElement(node);

            return node;
        }
        
        private Group CreateGroup(Vector2 startingPosition)
        {
            Group group = new Group() {
                title = DefaultGroupName
            };

            group.SetPosition(new Rect(startingPosition, Vector2.zero));

            AddElement(group);

            foreach (GraphElement element in selection)
            {
                if (element is not BaseNode)
                {
                    continue;
                }

                BaseNode node = (BaseNode)element;

                group.AddElement(node);
            }

            return group;
        }

        private void AddGridBackground()
        {
            GridBackground background = new GridBackground();

            Insert(0, background);
        }

        private void AddStyles()
        {
            StyleSheet graphStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueGraphStyle.uss");
            StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueNodeStyle.uss");

            styleSheets.Add(graphStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }

        private Vector2 GetLocalMousePosition(Vector2 mousePosition)
        {
            Vector2 mousePositionWorld = mousePosition;
            return contentViewContainer.WorldToLocal(mousePositionWorld);
        }
    }
}
