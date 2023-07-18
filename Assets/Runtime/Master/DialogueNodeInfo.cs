using Chocolate4.Dialogue.Runtime.Saving;
using System.Collections.Generic;

namespace Chocolate4.Dialogue.Runtime
{
    public class DialogueNodeInfo
    {
        public bool SituationEnded { get; private set; }
        public bool IsChoiceNode { get; private set; }
        public bool IsDialogueNode { get; private set; }
        /// <summary>Text of the node if <see cref="IsDialogueNode"/></summary>
        public string DialogueText { get; private set; }
        /// <summary>List of choices if <see cref="IsChoiceNode"/></summary>
        public List<string> Choices { get; private set; }
        /// <summary>DialogueEntity defined in the editor if <see cref="IsDialogueNode"/></summary>
        public DialogueEntity Speaker { get; private set; }

        public static DialogueNodeInfo SituationEndedInfo() => new DialogueNodeInfo() 
        { 
            SituationEnded = true 
        };

        public static DialogueNodeInfo ChoiceNodeInfo(List<string> choices) => new DialogueNodeInfo() 
        { 
            IsChoiceNode = true, Choices  = choices 
        };
        
        public static DialogueNodeInfo DialogueInfo(string text, DialogueEntity speaker) => new DialogueNodeInfo() 
        { 
            IsDialogueNode = true, DialogueText = text, Speaker = speaker 
        };
    }
}