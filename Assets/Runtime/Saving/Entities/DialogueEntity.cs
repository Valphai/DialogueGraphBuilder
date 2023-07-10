using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    public class DialogueEntity : ScriptableObject
    {
        public Sprite entityImage;
        public string entityName = string.Empty;
        public Sprite[] extraImages;
        public string[] extraText;
    }
}