using System;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    public interface IEntityProcessor
    {
        DialogueEntity Process(DialogueMaster master, DialogueEntity speaker);
    }
}