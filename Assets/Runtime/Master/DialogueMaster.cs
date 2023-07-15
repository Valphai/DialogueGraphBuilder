using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Runtime.Utilities;
using Chocolate4.Runtime.Utilities.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime
{
    [DisallowMultipleComponent]
    public class DialogueMaster : MonoBehaviour
    {
        [SerializeField] 
        private DialogueEditorAsset dialogueAsset;
        [SerializeField]
        private bool autoInitialize;

        private IDataHolder currentNode;
        private SituationSaveData currentSituation;
        private bool hasInitialized;
        private List<IDataHolder> allNodes;
        private int selectedChoice;
        private ParseAdapter parseAdapter;

        /// <summary>Collection of variables and events defined in the editor.</summary>
        public IDialogueMasterCollection Collection { get; private set; }

        public string CurrentSituationName => FindSituationName(currentSituation);

        private void Awake()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Starts a given situation at StartNode. To proceed to next node you have to explicitly invoke <seealso cref="NextDialogueElement"/>.
        /// </summary>
        /// <param name="situationName"></param>
        public void StartSituation(string situationName)
        {
            if (!hasInitialized)
            {
                Debug.LogError("Dialogue Master has not initialized yet!", this);
                return;
            }

            if (currentSituation != null)
            {
                Debug.Log($"There was an active situation: {currentSituation.situationId}.\nTrying to start new situation: {situationName}");
            }

            currentSituation = FindSituationByName(situationName);

            if (currentSituation == null)
            {
                Debug.LogError($"No situation was found with the name: {situationName}.");
                return;
            }

            currentNode = currentSituation.nodeData.Find(node => node.nodeType.Contains(NodeConstants.StartNode));

            allNodes =
                TypeExtensions.MergeFieldListsIntoOneImplementingType<IDataHolder, SituationSaveData>(currentSituation);
        }

        /// <summary>
        /// Gets next dialogue element as set in the editor graph.
        /// </summary>
        /// <returns>Information about the next dialogue element in the <seealso cref="DialogueNodeInfo"/> format.</returns>
        public DialogueNodeInfo NextDialogueElement()
        {
            if (!hasInitialized)
            {
                Debug.LogError("Tried to get next dialogue element but DialogueMaster was not initialized! This is not allowed.", this);
                return null;
            }

            if (currentSituation == null)
            {
                Debug.LogError("Tried to get next dialogue element but current situation was null! This is not allowed.");
                return null;
            }

            currentNode = NextNode();
            while (currentNode.IsNodeOfType(
                NodeConstants.ConditionNode, NodeConstants.ExpressionNode, 
                NodeConstants.FromSituationNode, NodeConstants.ToSituationNode,
                NodeConstants.EventPropertyNode)
            )
            {
                currentNode = NextNode();
            }

            if (currentNode.IsNodeOfType(NodeConstants.EndNode))
            {
                return DialogueNodeInfo.SituationEndedInfo();
            }
            else if (currentNode.IsNodeOfType(NodeConstants.DialogueNode))
            {
                DialogueNodeSaveData dialogueNodeSaveData = (currentNode as DialogueNodeSaveData);
                return DialogueNodeInfo.DialogueInfo(dialogueNodeSaveData.text, dialogueNodeSaveData.speaker);
            }
            else if (currentNode.IsNodeOfType(NodeConstants.ChoiceNode))
            {
                List<Nodes.DialogueChoice> choices = (currentNode as ChoiceNodeSaveData).choices;
                return DialogueNodeInfo.ChoiceNodeInfo(choices.Select(choice => choice.text).ToList());
            }

            return DialogueNodeInfo.SituationEndedInfo();
        }

        /// <summary>
        /// When the player receives a choice node, this method needs to be used before they make the decision.
        /// </summary>
        /// <param name="index">Index of selected choice.</param>
        public void SetSelectedChoice(int index)
        {
            selectedChoice = index;
        }

        public void Initialize()
        {
            Collection = CreateCollection();
            parseAdapter = new ParseAdapter(Collection);

            hasInitialized = true;
        }

        private IDialogueMasterCollection CreateCollection()
        {
            List<Type> collectionTypes = 
                TypeExtensions.GetTypes<IDialogueMasterCollection>(FilePathConstants.Chocolate4).ToList();

            string collectionName = FilePathConstants.GetCollectionName(dialogueAsset.fileName);
            Type assetMatchedCollectionType  = 
                collectionTypes.Find(type => type.Name.Equals(collectionName));

            return (IDialogueMasterCollection)Activator.CreateInstance(assetMatchedCollectionType);
        }

        private IDataHolder NextNode()
        {
            List<PortData> outputPortDataCollection = currentNode.NodeData.outputPortDataCollection;

            string nodeType = currentNode.GetNodeType();

            if (NodeUtilities.IsNodeOfType(nodeType,
                NodeConstants.StartNode, 
                NodeConstants.DialogueNode, NodeConstants.FromSituationNode)
            )
            {
                return FindNode(outputPortDataCollection.First().otherNodeID);
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.ChoiceNode))
            {
                return HandleChoiceNode(outputPortDataCollection);
            }
            else if (NodeUtilities.IsNodeOfType(nodeType,  NodeConstants.ConditionNode))
            {
                return HandleConditionNode(outputPortDataCollection);
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.ExpressionNode))
            {
                return HandleExpressionNode(outputPortDataCollection);
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.ToSituationNode))
            {
                return HandleToSituationNode();
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.EventPropertyNode))
            {
                return HandleEventPropertyNode(outputPortDataCollection);
            }

            Debug.LogError($"Encountered unsupported node {currentNode}! This node is null.");
            return null;
        }

        private IDataHolder HandleEventPropertyNode(List<PortData> outputPortDataCollection)
        {
            string eventName = FindPropertyNameById(((PropertyNodeSaveData)currentNode).propertyID);
            Collection.CollectionType.GetMethod(
                $"Invoke{eventName}", BindingFlags.NonPublic | BindingFlags.Instance
            ).Invoke(Collection, null);

            return FindNode(outputPortDataCollection.First().otherNodeID);
        }

        private IDataHolder HandleToSituationNode()
        {
            string currentSituationId = currentSituation.Id;
            string nextSituation = FindSituationName(((SituationTransferNodeSaveData)currentNode).otherSituationId);
            StartSituation(nextSituation);

            IDataHolder fromNode = FindNode<SituationTransferNodeSaveData>(node => node.otherSituationId.Equals(currentSituationId));
            return fromNode ?? FindNode<NodeSaveData>(node => node.IsNodeOfType(NodeConstants.StartNode));
        }

        private IDataHolder HandleExpressionNode(List<PortData> outputPortDataCollection)
        {
            parseAdapter.EvaluateSetExpressions(((TextNodeSaveData)currentNode).text);

            return FindNode(outputPortDataCollection.First().otherNodeID);
        }

        private IDataHolder HandleConditionNode(List<PortData> outputPortDataCollection)
        {
            if (parseAdapter.EvaluateConditions(((TextNodeSaveData)currentNode).text))
            {
                return FindNode(outputPortDataCollection.First().otherNodeID);
            }

            return FindNode(outputPortDataCollection.Last().otherNodeID);
        }

        private IDataHolder HandleChoiceNode(List<PortData> outputPortDataCollection)
        {
            int count = outputPortDataCollection.Count;
            if (count == 1)
            {
                return FindNode(outputPortDataCollection.First().otherNodeID);
            }

            if (selectedChoice > count)
            {
                Debug.LogError($"Selected choice index {selectedChoice} was greater than number of choices {count}! Aborting.");
                return null;
            }

            return FindNode(outputPortDataCollection[selectedChoice].otherNodeID);
        }

        private IDataHolder FindNode<T>(Predicate<T> match) where T : IDataHolder
        {
            if (currentSituation == null)
            {
                return null;
            }

            return TypeExtensions.GetListOfTypeFrom<T, SituationSaveData>(currentSituation).Find(match);
        }

        private IDataHolder FindNode(string id)
        {
            if (currentSituation == null)
            {
                return null;
            }

            return allNodes.Find(node => node.NodeData.nodeId == id);
        }

        private string FindPropertyNameById(string id)
        {
            return dialogueAsset.graphSaveData.blackboardSaveData.
                dialoguePropertiesSaveData.Find(property => property.id.Equals(id)).displayName;
        }

        private SituationSaveData FindSituationByName(string situationName)
        {
            TreeItemSaveData matchedData = 
                dialogueAsset.treeSaveData.treeItemData.Find(itemData => itemData.rootItem.displayName.Equals(situationName));

            return dialogueAsset.graphSaveData.situationSaveData.Find(data => data.Id == matchedData.rootItem.id);
        }

        private string FindSituationName(string id)
        {
            TreeItemSaveData matchedData =
                dialogueAsset.treeSaveData.treeItemData.Find(itemData => itemData.rootItem.id.Equals(id));

            if (matchedData == null)
            {
                Debug.LogWarning("Current situation is invalid.");
                return string.Empty;
            }

            return matchedData.rootItem.displayName;
        }

        private string FindSituationName(SituationSaveData saveData) => FindSituationName(saveData.Id);
    }
}