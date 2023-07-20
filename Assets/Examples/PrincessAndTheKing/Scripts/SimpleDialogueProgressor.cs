using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Runtime.Master.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chocolate4.Dialogue.Examples
{
    public class SimpleDialogueProgressor : MonoBehaviour
    {
        [SerializeField]
        protected DialogueMaster dialogueMaster;
        [SerializeField]
        protected Button progressButton;
        [SerializeField]
        private Button startConversationButton;

        public event Action<DialogueNodeInfo> OnReceivedDialogueInfo;
        public event Action<DialogueNodeInfo> OnReceivedSituationEndedInfo;

        private void Start()
        {
            progressButton.onClick.AddListener(() => ProgressInDialogue());
            startConversationButton.onClick.AddListener(() => StartSituation());

            DisplayStartSituationButton();
        }

        public virtual void StartSituation()
        {
            dialogueMaster.StartSituation(SimpleProgressionCollection.PrincessApproachesKing);
            HideStartConversationButton();
        }

        protected virtual DialogueNodeInfo ProgressInDialogue()
        {
            DialogueNodeInfo nextNodeInfo = dialogueMaster.NextDialogueElement();

            if (nextNodeInfo == null)
            {
                return null;
            }

            if (nextNodeInfo.SituationEnded)
            {
                DisplayStartSituationButton();
                OnReceivedSituationEndedInfo?.Invoke(nextNodeInfo);
                Debug.Log("Situation Ended!");
            }

            if (nextNodeInfo.IsDialogueNode)
            {
                OnReceivedDialogueInfo?.Invoke(nextNodeInfo);
            }

            return nextNodeInfo;
        }

        protected void HideStartConversationButton()
        {
            startConversationButton.gameObject.SetActive(false);
            progressButton.gameObject.SetActive(true);
        }

        protected void DisplayStartSituationButton()
        {
            startConversationButton.gameObject.SetActive(true);
            progressButton.gameObject.SetActive(false);
        }
    }
}
