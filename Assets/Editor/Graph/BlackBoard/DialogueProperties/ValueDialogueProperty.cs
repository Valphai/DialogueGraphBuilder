using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public abstract class ValueDialogueProperty<T> : IDialogueProperty, IExpandableDialogueProperty
    {
        public T Value { get; set; }

        private string displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                    return Id.ToString();
                return displayName;
            }
            set { displayName = value; }
        }

        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public abstract PropertyType PropertyType { get; protected set; }

        public abstract IConstantViewControlCreator ToConstantView();
        public abstract void UpdateConstantView();

        public virtual DialoguePropertySaveData Save()
        {
            return new DialoguePropertySaveData()
            {
                value = Value.ToString(),
                displayName = DisplayName,
                id = Id,
                propertyType = PropertyType,
            };
        }

        public virtual void Load(DialoguePropertySaveData saveData)
        {
            Value = (T)Convert.ChangeType(saveData.value, typeof(T));
            DisplayName = saveData.displayName;
            Id = saveData.id;
            PropertyType = saveData.propertyType;
        }
    }
}