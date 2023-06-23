using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class SituationTransferNode : BaseNode
    {
        private PopupField<string> popupField;

        public string NextSituationId { get; private set; } = string.Empty;
        public override NodeTask NodeTask { get; set; } = NodeTask.Transfer;

        public override bool CanConnectTo(BaseNode node, Direction direction)
        {
            return true;
        }

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new SituationTransferNodeSaveData() { nextSituationId = NextSituationId, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            SituationTransferNodeSaveData situationSaveData = (SituationTransferNodeSaveData)saveData;
            NextSituationId = situationSaveData.nextSituationId;

            IEnumerable<DialogueTreeItem> situations = DialogueEditorWindow.Window.DialogueTreeView.Situations;
            popupField.value = situations.First(treeItem => treeItem.guid.Equals(NextSituationId)).displayName;
        }

        protected override void DrawTitle()
        {
            base.DrawTitle();
            titleContainer.WithTransferStyle();
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
                NextSituationId = situations.First(item => item.displayName == selectedSituationName).guid;
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
