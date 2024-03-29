using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Entities.Utilities
{
    public static class EntitiesUtilities
    {
        public static Texture2D GetEntityImage(DialogueEntity entity)
        {
            return entity == null || entity.entityImage == null 
                ? ((Texture2D)EditorGUIUtility.Load(FilePathConstants.GetEditorVisualAssetPath(FilePathConstants.placeholderEntityPath)))
                : entity.entityImage.texture;
        }

        public static string GetEntityName(DialogueEntity entity, string[] existingNames = null)
        {
            return string.IsNullOrEmpty(entity.entityName) 
                ? ObjectNames.GetUniqueName(existingNames, EntitiesConstants.DefaultEntityName)
                : entity.entityName;
        }
    }
}