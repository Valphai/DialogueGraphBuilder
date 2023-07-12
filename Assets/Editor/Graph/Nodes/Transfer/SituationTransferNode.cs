using Chocolate4.Dialogue.Edit.Tree;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocolate4.Dialogue.Edit.Graph.Nodes
{
    public abstract class SituationTransferNode : BaseNode
    {
        private PopupField<string> popupField;
        private string selectedName;

        public string NextSituationId { get; private set; } = string.Empty;

        public override IDataHolder Save()
        {
            NodeSaveData saveData = (NodeSaveData)base.Save();
            return new SituationTransferNodeSaveData() { otherSituationId = NextSituationId, nodeSaveData = saveData };
        }

        public override void Load(IDataHolder saveData)
        {
            base.Load(saveData);
            SituationTransferNodeSaveData situationSaveData = (SituationTransferNodeSaveData)saveData;
            NextSituationId = situationSaveData.otherSituationId;

            IEnumerable<DialogueTreeItem> situations = DialogueEditorWindow.Window.DialogueTreeView.DialogueTreeItems;
            
            DialogueTreeItem selectedSituation;
            try
            {
                selectedSituation = situations.First(treeItem => treeItem.id.Equals(NextSituationId));
            }
            catch (System.InvalidOperationException)
            {
                selectedSituation = situations.First();
                Debug.Log($"Removed situation with id: {NextSituationId} was navigated to by {this}. Changing the situation to: {selectedSituation.displayName}");
                NextSituationId = selectedSituation.id;
            }

            popupField.value = selectedSituation.displayName;
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
            IEnumerable<DialogueTreeItem> situations = dialogueTreeView.DialogueTreeItems;
            List<string> situationNames = situations.Select(treeItem => treeItem.displayName).ToList();

            popupField = new PopupField<string>(situationNames, 0, selectedSituationName => {
                selectedName = selectedSituationName;
                NextSituationId = situations.First(item => item.displayName == selectedSituationName).id;
                return selectedSituationName;
            });

            return dialogueTreeView;
        }

        private void DialogueTreeView_OnTreeItemRenamed(string changedId)
        {
            DialogueTreeView dialogueTreeView = DialogueEditorWindow.Window.DialogueTreeView;

            List<DialogueTreeItem> situations = dialogueTreeView.DialogueTreeItems.ToList();
            List<string> situationNames = situations.Select(treeItem => treeItem.displayName).ToList();

            foreach (DialogueTreeItem situation in situations)
            {
                if (situation.id == changedId && !situation.displayName.Equals(selectedName))
                {
                    selectedName = situation.displayName;
                    popupField.SetValueWithoutNotify(selectedName);
                    return;
                }
            }

            popupField.choices = situationNames;
        }
    }
}
