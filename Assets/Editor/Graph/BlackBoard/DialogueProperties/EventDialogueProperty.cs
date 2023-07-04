using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.BlackBoard
{
    [Serializable]
    public class EventDialogueProperty : IDialogueProperty, IDraggableProperty
    {
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
        public PropertyType PropertyType { get; protected set; } = PropertyType.Event;

        public EventDialogueProperty()
        {
            DisplayName = PropertyType.ToString();
        }

        public virtual DialoguePropertySaveData Save()
        {
            return new DialoguePropertySaveData()
            {
                displayName = DisplayName,
                id = Id,
                propertyType = PropertyType,
            };
        }

        public virtual void Load(DialoguePropertySaveData saveData)
        {
            DisplayName = saveData.displayName;
            Id = saveData.id;
            PropertyType = saveData.propertyType;
        }

        public BaseNode ToConcreteNode() => new EventPropertyNode()
        {
            Name = DisplayName,
            PropertyId = Id,
        };
    }
}