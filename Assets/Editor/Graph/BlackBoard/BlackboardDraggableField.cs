using Chocolate4.Edit.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
	class BlackboardDraggableField : BlackboardField
	{
        [SerializeField] 
        private DialogueGraphView dialogueGraphView;

        public BlackboardDraggableField(DialogueGraphView dialogueGraphView, string text, string typeText) : base(null, text, typeText)
        {
            this.dialogueGraphView = dialogueGraphView;
            RegisterCallback<DragLeaveEvent>(DragHandler);
        }

        private void DragHandler(DragLeaveEvent evt)
        {
            dialogueGraphView.DragSelectablesHandler.RegisterSelectedProperty(userData as IDialogueProperty);
        }
    }
}