using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Edit.Entities.Utilities
{
    public static class EntitiesUtilities
    {
        public static Texture2D GetEntityImage(DialogueEntity entity)
        {
            return entity.entityImage == null 
                ? ((Texture2D)EditorGUIUtility.Load(FilePathConstants.placeholderEntityPath))
                : entity.entityImage.texture;
        }

        public static string GetEntityName(DialogueEntity entity, string[] existingNames = null)
        {
            return string.IsNullOrEmpty(entity.entityName) ? ScriptableObjectUtilities.GetUniqueNameFromPath(
                FilePathConstants.dialogueEntitiesPath, EntitiesConstants.DefaultEntityName, existingNames
            ) : entity.entityName;
        }
    }
}