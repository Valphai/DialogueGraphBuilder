using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    [System.Serializable]
    public class GroupSaveData
    {
        [field: SerializeField]
        public string ID { get; set; }
        [field: SerializeField]
        public string Name { get; set; }
        [field: SerializeField]
        public Vector2 Position { get; set; }
    }
}