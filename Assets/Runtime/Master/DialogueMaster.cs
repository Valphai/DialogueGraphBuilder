using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Runtime.Utilities;
using Chocolate4.Runtime.Utilities.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime
{
    [DisallowMultipleComponent]
    public class DialogueMaster : MonoBehaviour
    {
        [SerializeField] 
        private DialogueEditorAsset dialogueAsset;

        private IDataHolder currentNode;
        private SituationSaveData currentSituation;
        private bool hasInitialized;
        private List<IDataHolder> allNodes;
        private int selectedChoice;
        private ParseAdapter parseAdapter;

        /// <summary>Collection of variables and events defined in the editor.</summary>
        public DialogueMasterCollection Collection { get; private set; }

        public string CurrentSituationName => FindSituationName(currentSituation);

        private void Awake()
        {
            Initialize();
            hasInitialized = true;
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
                NodeConstants.FromSituationNode, NodeConstants.ToSituationNode)
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
                return DialogueNodeInfo.DialogueInfo((currentNode as TextNodeSaveData).text);
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

        private void Initialize()
        {
            Collection = new DialogueMasterCollection();
            parseAdapter = new ParseAdapter(Collection);
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
            else if (NodeUtilities.IsNodeOfType(nodeType,  NodeConstants.ConditionNode))
            {
                if (parseAdapter.EvaluateConditions(((TextNodeSaveData)currentNode).text))
                {
                    return FindNode(outputPortDataCollection.First().otherNodeID);
                }

                return FindNode(outputPortDataCollection.Last().otherNodeID);
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.ExpressionNode))
            {
                parseAdapter.EvaluateSetExpressions(((TextNodeSaveData)currentNode).text);

                return FindNode(outputPortDataCollection.First().otherNodeID);
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.ToSituationNode))
            {
                string currentSituationId = currentSituation.Id;
                string nextSituation = FindSituationName(((SituationTransferNodeSaveData)currentNode).otherSituationId);
                StartSituation(nextSituation);

                return FindNode<SituationTransferNodeSaveData>(node => node.otherSituationId.Equals(currentSituationId));
            }
            else if (NodeUtilities.IsNodeOfType(nodeType, NodeConstants.EventPropertyNode))

            Debug.LogError($"Encountered unsupported node {currentNode}! This node is null.");
            return null;
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