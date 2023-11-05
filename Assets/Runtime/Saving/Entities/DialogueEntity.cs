using Chocolate4.Dialogue.Runtime.Common;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Saving
{
    public class DialogueEntity : ScriptableObject, ICloneable, IDisposable
    {
        public Sprite entityImage;
        public string entityName = string.Empty;
        public Sprite[] extraImages;
        public string[] extraText;

        private bool original = true;

        [field:SerializeField, HideInInspector]
        public string Identifier { get; private set; }

        [InterfaceList, SerializeReference, NonReorderable]
        public List<IEntityProcessor> entityProcessors = new List<IEntityProcessor>();

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            if (!string.IsNullOrEmpty(Identifier))
            {
                return;
            }

            Identifier = Guid.NewGuid().ToString();
        }

        public object Clone()
        {
            DialogueEntity clone = CreateInstance<DialogueEntity>();

            clone.Identifier = Identifier;
            clone.entityImage = entityImage;
            clone.entityName = entityName;
            clone.extraImages = extraImages;
            clone.extraText = extraText;
            clone.entityProcessors = entityProcessors.ToList();
            clone.original = false;
            return clone;
        }

        public void Dispose()
        {
            if (original)
            {
                Debug.LogWarning("Tried to dispose of original entity! Operation aborted.");
                return;
            }

            Destroy(this); 
        }

        public DialogueEntity Process(DialogueMaster master)
        {
            DialogueEntity processedEntity = (DialogueEntity)Clone();
            if (entityProcessors.IsNullOrEmpty())
            {
                return processedEntity;
            }

            foreach (var entityProcessor in entityProcessors)
            {
                processedEntity = entityProcessor.Process(master, processedEntity);
            }

            return processedEntity;
        }
    }
}