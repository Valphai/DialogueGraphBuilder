using Chocolate4.Dialogue.Runtime;
using System;

namespace Chocolate4.Dialogue.Examples
{
    public abstract class AdvancedDialogueProgressor : SimpleDialogueProgressor
    {
        public event Action<DialogueNodeInfo> OnReceivedChoiceInfo;

        public void SetChoice(int index)
        {
            dialogueMaster.SetSelectedChoice(index);
            ProgressInDialogue();
        }

        protected override DialogueNodeInfo ProgressInDialogue()
        {
            DialogueNodeInfo nextNodeInfo = base.ProgressInDialogue();
            progressButton.enabled = true;

            if (nextNodeInfo.IsChoiceNode)
            {
                OnReceivedChoiceInfo?.Invoke(nextNodeInfo);
                progressButton.enabled = false;
            }

            return nextNodeInfo;
        }
    }
}
