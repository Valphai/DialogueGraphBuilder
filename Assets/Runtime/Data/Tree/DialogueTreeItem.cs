using System;

namespace Chocolate4.Dialogue.Edit.Tree
{
    [Serializable]
    public class DialogueTreeItem
    {
        public string displayName;
        public string id;

        public DialogueTreeItem(string name)
        {
            id = Guid.NewGuid().ToString();
            displayName = name;
        }
    }
}
