using Chocolate4.Dialogue.Edit.Entities;
using Chocolate4.Dialogue.Edit.Utilities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Edit.Entities.Utilities
{
    public static class EntitiesUtilities
    {
        public static Texture2D GetEntityImage(DialogueEntity entity)
        {
            return entity == null || entity.entityImage == null 
                ? ((Texture2D)EditorGUIUtility.Load(FilePathConstants.placeholderEntityPath))
                : entity.entityImage.texture;
        }

        public static string GetEntityName(DialogueEntity entity, string[] existingNames = null)
        {
            return string.IsNullOrEmpty(entity.entityName) 
                ? ObjectNames.GetUniqueName(existingNames, EntitiesConstants.DefaultEntityName)
                : entity.entityName;
        }

        internal static List<DialogueEntity> GetAllEntities(EntitiesHolder dataBase, out string folderPath)
        {
            List<DialogueEntity> allEntities = new List<DialogueEntity>();

            int instanceId = dataBase.GetInstanceID();

            string assetPath = AssetDatabase.GetAssetPath(instanceId);
            string assetDirectoryPath = Path.GetDirectoryName(assetPath);

            string fileName = Path.GetFileName(assetPath).Replace("." + FilePathConstants.Extension, string.Empty);

            string identifiedFolderName = fileName + FilePathConstants.EntitiesFolder;

            folderPath = assetDirectoryPath + FilePathConstants.DirSep + identifiedFolderName;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(assetDirectoryPath, identifiedFolderName);
                return allEntities;
            }

            string[] entitiesPaths = Directory.GetFiles(folderPath, "*.asset");

            foreach (string path in entitiesPaths)
            {
                DialogueEntity entity = AssetDatabase.LoadAssetAtPath<DialogueEntity>(path);
                allEntities.Add(entity);
            }

            return allEntities;
        }
    }
}