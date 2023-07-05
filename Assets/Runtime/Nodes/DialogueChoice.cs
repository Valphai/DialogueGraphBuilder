using Chocolate4.Dialogue.Runtime.Utilities;

namespace Chocolate4.Dialogue.Runtime.Nodes
{
    [System.Serializable]
    public class DialogueChoice
    {
        public string name;
        public string text;

        public int ChoiceIndex => StringExtensions.GetMemberIndex(name);
    }
}
