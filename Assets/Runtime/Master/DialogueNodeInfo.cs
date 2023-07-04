using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime
{
    public class DialogueNodeInfo
    {
        public bool SituationEnded { get; private set; }
        public bool IsChoiceNode { get; private set; }
        public bool IsDialogueNode { get; private set; }
        public string DialogueText { get; private set; }
        public List<string> Choices { get; private set; }

        public static DialogueNodeInfo SituationEndedInfo() => new DialogueNodeInfo() { SituationEnded = true };

        public static DialogueNodeInfo ChoiceNodeInfo(List<string> choices) => new DialogueNodeInfo() { IsChoiceNode = true, Choices  = choices };
        
        public static DialogueNodeInfo DialogueInfo(string text) => new DialogueNodeInfo() { IsDialogueNode = true, DialogueText = text };
    }
}