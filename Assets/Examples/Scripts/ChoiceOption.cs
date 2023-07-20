using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chocolate4.Dialogue.Examples
{
    [RequireComponent(typeof(Button))]
    public class ChoiceOption : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text choiceText;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void SetButton(string choice, int choiceIndex, Action<int> onClick)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick(choiceIndex));
            choiceText.SetText(choice);
        }
    }
}
