using System;
using UnityEngine.UIElements;

namespace Chocolate4
{
    [Serializable]
    public class DialogueTreeItem
    {
        public string displayName;
        public readonly string guid;
        public readonly string prefix; // TODO: make an icon for this
        public readonly Func<DialogueTreeItem, VisualElement> makeItem;

        public DialogueTreeItem(string name, string prefix, Func<DialogueTreeItem, VisualElement> makeItem)
        {
            guid = Guid.NewGuid().ToString();
            displayName = name;
            this.prefix = prefix;
            this.makeItem = makeItem;
        }
    }
}
