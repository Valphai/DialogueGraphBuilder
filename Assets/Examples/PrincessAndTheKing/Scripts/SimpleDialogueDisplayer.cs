using Chocolate4.Dialogue.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chocolate4.Dialogue.Examples
{
    public class SimpleDialogueDisplayer : MonoBehaviour
    {
        [SerializeField]
        private float letterDelay = 0.1f;
        [SerializeField]
        private Image speakerImage;
        [SerializeField]
        private TMP_Text speakerNameText;
        [SerializeField]
        private TMP_Text displayText;
        [SerializeField]
        private SimpleDialogueProgressor dialogueProgressor;

        private string fullText;

        private void OnEnable()
        {
            dialogueProgressor.OnReceivedDialogueInfo += DialogueProgressor_OnReceivedDialogueInfo;
        }
        
        private void OnDisable()
        {
            dialogueProgressor.OnReceivedDialogueInfo -= DialogueProgressor_OnReceivedDialogueInfo;
        }

        private void Start()
        {
            displayText.SetText(string.Empty);
            speakerNameText.SetText(string.Empty);
        }

        private void DialogueProgressor_OnReceivedDialogueInfo(DialogueNodeInfo dialogueInfo)
        {
            speakerImage.sprite = dialogueInfo.Speaker.entityImage;
            speakerNameText.SetText(dialogueInfo.Speaker.name);
            fullText = dialogueInfo.DialogueText;

            BreakPreviousReveal();
            StartCoroutine(RevealText());
        }

        private void BreakPreviousReveal()
        {
            displayText.SetText(string.Empty);
            StopAllCoroutines();
        }

        private IEnumerator RevealText()
        {
            for (int i = 0; i < fullText.Length; i++)
            {
                displayText.text += fullText[i];
                yield return new WaitForSeconds(letterDelay);
            }
        }
    } 
}