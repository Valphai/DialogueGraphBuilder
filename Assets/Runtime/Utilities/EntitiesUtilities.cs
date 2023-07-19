using Chocolate4.Dialogue.Runtime.Entities;
using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime
{
    public static class EntitiesUtilities
    {
        public static List<DialogueEntity> GetAllEntities(EntitiesHolder dataBase)
        {
            List<DialogueEntity> allEntities = new List<DialogueEntity>();

            string associatedAssetName = dataBase.associatedAssetName;
            string identifiedFolderName = associatedAssetName + FilePathConstants.EntitiesFolder;

            string folderPath = FilePathConstants.Chocolate4 + FilePathConstants.DirSep + identifiedFolderName;

#if UNITY_EDITOR
            string assetsRelativePath = FilePathConstants.GetEntitiesPathRelative(associatedAssetName);

            if (!AssetDatabase.IsValidFolder(assetsRelativePath))
            {
                string relativePathNoFolderName = assetsRelativePath.Replace(FilePathConstants.DirSep + identifiedFolderName + FilePathConstants.DirSep, string.Empty);
                AssetDatabase.CreateFolder(relativePathNoFolderName, identifiedFolderName);
                return allEntities;
            }
#endif
            allEntities.AddRange(Resources.LoadAll<DialogueEntity>(folderPath));
            return allEntities;
        }
    }
}
