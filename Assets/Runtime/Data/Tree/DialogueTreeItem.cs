using System;

namespace Chocolate4.Dialogue.Edit.Tree
{
    [Serializable]
    public class DialogueTreeItem
    {
        public string displayName;
        public string guid;
        public string prefix; // TODO: make an icon for this

        public DialogueTreeItem(string name, string prefix)
        {
            guid = Guid.NewGuid().ToString();
            displayName = name;
            this.prefix = prefix;
        }
    }
}
