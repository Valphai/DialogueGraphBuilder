using System;

namespace Chocolate4.Dialogue.Edit.Tree
{
    [Serializable]
    public class DialogueTreeItem
    {
        public string displayName;
        public string id;
        public string prefix; // TODO: make an icon for this

        public DialogueTreeItem(string name, string prefix)
        {
            id = Guid.NewGuid().ToString();
            displayName = name;
            this.prefix = prefix;
        }
    }
}
