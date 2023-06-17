using Chocolate4.Dialogue.Edit.Graph.Nodes;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public abstract class DialogueProperty<T> : IDialogueProperty
    {
        public T Value { get; set; }

        private string displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                    return Guid.ToString();
                return displayName;
            }
            set { displayName = value; }
        }

        public string Guid { get; } = System.Guid.NewGuid().ToString();

        public abstract PropertyType PropertyType { get; }

        public abstract BaseNode ToConcreteNode();
    }
}