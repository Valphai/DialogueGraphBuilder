using Chocolate4.Dialogue.Edit.Tree;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public class SituationNode : BaseNode
    {
        private PopupField<string> popupField;

        public string NextSituationGuid { get; private set; } = string.Empty;
        public override string Name { get; set; } = "Situation Node";
        public override NodeTask NodeTask { get; set; } = NodeTask.Dialogue;

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return true;
        }

        public override void Initialize(Vector3 startingPosition)
        {
            base.Initialize(startingPosition);
            Name = "Next Situation";
        }

        protected override void DrawPorts()
        {
            DrawInputPort();
        }

        protected override void AddExtraContent(VisualElement contentContainer)
        {
            DialogueTreeView dialogueTreeView = DialogueEditorWindow.Window.DialogueTreeView;
            CreatePopup(dialogueTreeView);

            dialogueTreeView.OnTreeItemRenamed += DialogueTreeView_OnTreeItemRenamed;

            contentContainer.Add(popupField);
        }

        private DialogueTreeView CreatePopup(DialogueTreeView dialogueTreeView)
        {
            IEnumerable<DialogueTreeItem> situations = dialogueTreeView.Situations;

            List<string> situationNames = situations.Select(treeItem => treeItem.displayName).ToList();
            
            popupField = new PopupField<string>(situationNames, 0, selectedSituationName => {
                NextSituationGuid = situations.First(item => item.displayName == selectedSituationName).guid;
                return selectedSituationName;
            }); 

            return dialogueTreeView;
        }

        private void DialogueTreeView_OnTreeItemRenamed(string changedGuid)
        {
            DialogueTreeView dialogueTreeView = DialogueEditorWindow.Window.DialogueTreeView;
            IEnumerable<DialogueTreeItem> situations = dialogueTreeView.Situations;

            List<string> situationNames = situations.Select(treeItem => treeItem.displayName).ToList();

            popupField.choices = situationNames;
        }
    }
}
