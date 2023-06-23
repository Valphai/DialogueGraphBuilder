using Chocolate4.Dialogue.Edit.Graph.BlackBoard;
using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Edit.Saving;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Edit.Graph.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Graph
{
    public class DialogueGraphView : GraphView, IRebuildable<GraphSaveData>
    {
        public const string DefaultGroupName = "Dialogue Group";

        private string activeSituationGuid = string.Empty;
        private BlackboardProvider blackboardProvider;

        public DragSelectablesHandler DragSelectablesHandler { get; private set; }
        internal SituationCache SituationCache { get; private set; }

        public void Initialize()
        {
            deleteSelection = CustomDeleteSelection;
            graphViewChanged = OnGraphViewChange;

            ResolveDependencies();

            AddManipulators();
            AddGridBackground();

            AddStyles();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction($"Add Group",
                actionEvent => CreateGroup(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))
            );

            List<Type> nodeTypes = TypeExtensions.GetTypes<BaseNode>()
                .Except(new Type[] { typeof(StartNode), typeof(EndNode) }).ToList();

            string contextElementTitle;
            foreach (Type nodeType in nodeTypes)
            {
                contextElementTitle = nodeType.Name;
                evt.menu.AppendAction($"Add {contextElementTitle}",
                    actionEvent => CreateNode(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), nodeType)
                );
            }

            if (evt.target is BaseNode)
            {
                evt.menu.AppendAction("Convert To Property", ConvertToProperty, ConvertToPropertyStatus);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port => {

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                BaseNode startNode = (BaseNode)startPort.node;
                BaseNode node = (BaseNode)port.node;

                if (startNode.IsConnectedTo(node))
                {
                    return;
                }

                if (!startNode.CanConnectTo(node, startPort.direction))
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public GraphSaveData Save()
        {
            CacheActiveSituation();
            return new GraphSaveData()
            {
                situationSaveData = SituationCache.Cache.ToList(),
                blackboardSaveData = blackboardProvider.Save()
            };
        }

        public void Rebuild(GraphSaveData graphSaveData)
        {
            DeleteElements(graphElements);
            RebuildGraph(graphSaveData);

            BlackboardSaveData blackboardSaveData = graphSaveData.blackboardSaveData;
            if (blackboardSaveData != null)
            {
                blackboardProvider.Rebuild(blackboardSaveData);
            }
        }

        internal void DialogueTreeView_OnSituationSelected(string newSituationGuid)
        {
            if (!activeSituationGuid.Equals(string.Empty))
            {
                CacheActiveSituation();
            }

            activeSituationGuid = newSituationGuid;

            DeleteElements(graphElements);
            if (SituationCache.IsCached(newSituationGuid, out SituationSaveData situationSaveData))
            {
                RebuildGraph(situationSaveData);
            }

            blackboardProvider.UpdatePropertyBinds();
        }

        internal void DialogueTreeView_OnTreeItemRemoved(string treeItemGuid)
        {
            if (!SituationCache.IsCached(treeItemGuid, out SituationSaveData cachedSituationSaveData))
            {
                return;
            }

            if (!SituationCache.TryRemove(cachedSituationSaveData))
            {
                Debug.LogError($"Situation {cachedSituationSaveData.situationId} could not be removed.");
                return;
            }
        }

        private void ResolveDependencies()
        {
            SituationCache = new SituationCache(null);
            DragSelectablesHandler = new DragSelectablesHandler();

            blackboardProvider = new BlackboardProvider(this);
            Add(blackboardProvider.Blackboard);
        }

        private void CacheActiveSituation()
        {
            SituationSaveData situationSaveData = 
                StructureSaver.SaveSituation(activeSituationGuid, graphElements);

            SituationCache.TryCache(situationSaveData);
        }

        private void RebuildGraph(GraphSaveData graphSaveData)
        {
            List<SituationSaveData> situationSaveData = graphSaveData.situationSaveData;
            if (situationSaveData.IsNullOrEmpty())
            {
                return;
            }

            SituationCache = new SituationCache(situationSaveData);

            SituationSaveData situationData = situationSaveData.Find(
                data => data.situationId.Equals(activeSituationGuid)
            );

            if (situationData == null)
            {
                return;
            }

            RebuildGraph(situationData);
        }

        private void RebuildGraph(SituationSaveData situationData)
        {
            if (!situationData.TryMergeDataIntoHolder(out List<IDataHolder> dataHolders))
            {
                AddStartingNodes();
                return;
            }

            List<BaseNode> nodes = new List<BaseNode>();
            List<Type> nodeTypes = TypeExtensions.GetTypes<BaseNode>().ToList();
            foreach (IDataHolder dataHolder in dataHolders)
            {
                Type matchedType = nodeTypes.First(type => type.ToString().Contains(dataHolder.NodeData.nodeType));
                BaseNode node = CreateNode(dataHolder.NodeData.position, matchedType);
                node.Load(dataHolder);
                nodes.Add(node);
            }

            RebuildConnections(nodes);
        }

        private void RebuildConnections(List<BaseNode> nodes)
        {
            foreach (BaseNode node in nodes)
            {
                foreach (PortData portData in node.OutputPortDataCollection)
                {
                    if (string.IsNullOrEmpty(portData.thisPortName))
                    {
                        continue;
                    }

                    string childID = portData.otherNodeID;
                    List<BaseNode> connections =
                        nodes.Where(childNode => childNode.ID == childID).ToList();

                    Port outputPort = node.outputContainer.Q<Port>(portData.thisPortName);

                    ConnectPorts(outputPort, portData, connections);
                }
            }
        }

        private void ConnectPorts(Port port, PortData portData, List<BaseNode> connections)
        {
            foreach (BaseNode otherNode in connections)
            {
                Port otherPort = otherNode.inputContainer.Q<Port>(portData.otherPortName);
                if (otherPort == null)
                {
                    continue;
                }

                Edge edge = port.ConnectTo(otherPort);

                AddElement(edge);
                otherNode.RefreshPorts();
            }
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new DragAndDropManipulator(this));
        }

        private void ConvertToProperty(DropdownMenuAction arg)
        {
            List<IPropertyNode> selectedPropertyNodes = selection.OfType<IPropertyNode>().ToList();
            List<Type> propertyTypes = TypeExtensions.GetTypes<IDialogueProperty>().ToList();

            foreach (IPropertyNode propertyNode in selectedPropertyNodes)
            {
                Type typeToMake = 
                    propertyTypes.First(type => type.ToString().Contains(propertyNode.PropertyType.ToString()));

                IDialogueProperty property = (IDialogueProperty)Activator.CreateInstance(typeToMake);

                blackboardProvider.AddProperty(property, true);
                propertyNode.BindToProperty(property);
            }
        }

        private DropdownMenuAction.Status ConvertToPropertyStatus(DropdownMenuAction arg)
        {
            if (selection.OfType<IPropertyNode>().Any(node => node.IsBoundToProperty))
            {
                return DropdownMenuAction.Status.Hidden;
            }

            return DropdownMenuAction.Status.Normal;
        }

        private BaseNode CreateNode(Vector2 startingPosition, Type nodeType)
        {
            BaseNode node = (BaseNode)Activator.CreateInstance(nodeType);
            AddNode(startingPosition, node);

            return node;
        }

        public void AddNode(Vector2 startingPosition, BaseNode node)
        {
            node.Initialize(startingPosition);
            node.Draw();
            node.PostInitialize();

            AddElement(node);
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

        private GraphViewChange OnGraphViewChange(GraphViewChange change)
        {
            if (change.elementsToRemove.IsNullOrEmpty())
            {
                return change;
            }

            foreach (GraphElement elementToRemove in change.elementsToRemove)
            {
                if (elementToRemove.userData is IDialogueProperty)
                {
                    blackboardProvider.HandlePropertyRemove(elementToRemove.userData as IDialogueProperty);
                    return change;
                }
            }

            return change;
        }

        private void CustomDeleteSelection(string operationName, AskUser askUser)
        {
            HashSet<GraphElement> toRemove = new HashSet<GraphElement>();
            foreach (ISelectable selectable in selection)
            {
                if (selectable is not GraphElement)
                {
                    continue;
                }

                if (selectable is StartNode or EndNode)
                {
                    continue;
                }

                toRemove.Add(selectable as GraphElement);
            }

            //graph.owner.RegisterCompleteObjectUndo(operationName);
            DeleteElements(toRemove);
        }

        private void AddStartingNodes()
        {
            Vector2 offset = Vector2.right * GraphConstants.StartingNodesOffset;
            Vector2 middlePoint = new Vector2(
                contentContainer.contentRect.width * .5f, contentContainer.contentRect.height * .5f
            );

            CreateNode(middlePoint - offset, typeof(StartNode));
            CreateNode(middlePoint + offset, typeof(EndNode));
        }
    }
}
