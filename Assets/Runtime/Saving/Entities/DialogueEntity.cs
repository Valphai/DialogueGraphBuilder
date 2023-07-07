using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    public class DialogueEntity : ScriptableObject
    {
        public Sprite entityImage;
        public string entityName;
        public Sprite[] extraImages;
        public string[] extraText;

        public string EntityName => string.IsNullOrEmpty(entityName) ? name : entityName;
    }
}