using Chocolate4.Editor;
using Chocolate4.Editor.Saving;
using Chocolate4.Saving;
using Chocolate4.Utilities;
using System;
using System.Collections.Generic;
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
        private List<SituationSaveData> situationToData = new List<SituationSaveData>();

        public void Initialize()
        {
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
            return StructureSaver.SaveGraph(situationToData);
        }

        public void Rebuild(GraphSaveData graphSaveData)
        {
            //return;
            if (graphSaveData.situationSaveData.IsNullOrEmpty())
            {
                return;
            }

            foreach (SituationSaveData situationSaveData in graphSaveData.situationSaveData)
            {
                situationToData.Add(situationSaveData);
            }

            SituationSaveData situationData = graphSaveData.situationSaveData.Find(
                data => data.situationGuid.Equals(activeSituationGuid)
            );

            if (situationData == null)
            {
                return;
            }

            RebuildGraph(situationData);
        }

        public void DialogueTreeView_OnSituationSelected(string newSituationGuid)
        {
            CacheActiveSituation();
            DeleteElements(graphElements);

            activeSituationGuid = newSituationGuid;
            SituationSaveData situationSaveData = situationToData.Find(
                situation => situation.situationGuid == newSituationGuid
            );

            if (situationSaveData == null)
            {
                return;
            }

            RebuildGraph(situationSaveData);
        }


        private SituationSaveData SaveActiveSituation()
        {
            Dictionary<BaseNode, List<BaseNode>> nodeToOtherNodes = SaveActiveGraph();
            return StructureSaver.SaveSituation(activeSituationGuid, nodeToOtherNodes);
        }

        private bool IsCached(string situationGuid, out SituationSaveData cachedSaveData)
        {
            cachedSaveData = situationToData.Find(
                situation => situation.situationGuid == situationGuid
            );

            return cachedSaveData != null;
        }

        private void CacheActiveSituation()
        {
            SituationSaveData situationSaveData = SaveActiveSituation();

            if (!IsCached(situationSaveData.situationGuid, out SituationSaveData _))
            {
                situationToData.Add(situationSaveData);
            }
        }

        private Dictionary<BaseNode, List<BaseNode>> SaveActiveGraph()
        {
            var nodeToOtherNodes = new Dictionary<BaseNode, List<BaseNode>>();

            graphElements.ForEach(element => {
                if (element is BaseNode node)
                {
                    Port inputPort = node.inputContainer.Q<Port>();
                    var connectionsMap = GetConnections(inputPort);

                    nodeToOtherNodes.Add(node, connectionsMap);
                }
            });

            return nodeToOtherNodes;
        }

        private void RebuildGraph(SituationSaveData situationData)
        {
            foreach (NodeSaveData nodeData in situationData.nodeData)
            {
                AddElement(nodeData.parentNode);
            }

            foreach (NodeSaveData nodeData in situationData.nodeData)
            {
                BaseNode node = nodeData.parentNode;
                Port inputPort = node.inputContainer.Q<Port>();
                List<BaseNode> connections = nodeData.parentInputs;

                ConnectPorts(inputPort, connections);
            }
        }

        private void ConnectPorts(Port port, List<BaseNode> connections)
        {
            foreach (BaseNode otherNode in connections)
            {
                Port otherPort = otherNode.outputContainer.Q<Port>();

                Edge edge = otherPort.ConnectTo(port);

                AddElement(edge);

                otherNode.RefreshPorts();
            }
        }

        private List<BaseNode> GetConnections(Port port)
        {
            IEnumerable<Edge> connections = port.connections;

            var connectionsMap = new List<BaseNode>();

            foreach (Edge connection in connections)
            {
                connectionsMap.Add(connection.output.node as BaseNode);
            }
            return connectionsMap;
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

            IEnumerable<Type> nodeTypes = TypeHelpers.GetTypes<BaseNode>();

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
