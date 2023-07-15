using System.IO;
using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Utilities
{
    public static class FilePathConstants
    {
        public const string Chocolate4 = "Chocolate4";
        public const string Assets = "Assets";
        public const string AssetFolder = "";
        public const string Collection = "Collection";
        public const string Extension = "chocolatedialogue";
        public const string EntitiesFolder = "Entities";

        public static string testCasesAssetPath = $"{Application.dataPath}{DirSep}{AssetFolder}{DirSep}Tests{DirSep}PlayMode{DirSep}Cases{DirSep}TestCasesDialogueEditor.{Extension}";
        
        public static string placeholderEntityPath = $"Icons{DirSep}Placeholder Entity 512.png";
        public static string situationIconPath = $"Icons{DirSep}Situation Gem 128.png";
        public static string assetIcon = $"Icons{DirSep}Book Icon 64.png";

        public static char DirSep => Path.DirectorySeparatorChar;

        public static string GetPathRelativeTo(string start, string path)
        {
            int lastIndex = path.LastIndexOf(start);
            return path.Substring(lastIndex);
        }

        public static string GetCollectionPath(string collectionName)
        {
            return Application.dataPath + DirSep + AssetFolder + "Runtime" + DirSep + "Master" + DirSep + "Collections" + DirSep + collectionName + ".cs";
        }

        public static string GetCollectionName(string fileName) => fileName + Collection;

        public static bool FileIsDuplicate(string path, string generatedFile)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            string existingFile = File.ReadAllText(path);
            if (existingFile != generatedFile)
            {
                return false;
            }

            return true;
        }
    }
}
