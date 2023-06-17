using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Edit.Graph
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private DialogueGraphView dialogueGraphView;

        public DragAndDropManipulator(DialogueGraphView dialogueGraphView)
        {
            this.dialogueGraphView = dialogueGraphView;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragUpdatedEvent>(DragUpdatedHandler);
            target.RegisterCallback<DragExitedEvent>(DragExitedHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragUpdatedEvent>(DragUpdatedHandler);
            target.UnregisterCallback<DragExitedEvent>(DragExitedHandler);
        }

        private void DragUpdatedHandler(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        }

        private void DragExitedHandler(DragExitedEvent evt)
        {
            Vector2 spawnPosition = evt.localMousePosition;
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

            BaseNode node = dialogueGraphView.DragSelectablesHandler.SelectedProperty.ToConcreteNode();

            dialogueGraphView.AddNode(spawnPosition, node);
        }
    }
}
