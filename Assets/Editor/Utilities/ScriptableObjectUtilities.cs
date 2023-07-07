using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Utilities
{
    public static class ScriptableObjectUtilities
    {
        private const string Asset = ".asset";

        public static T CreateAssetAtPath<T>(string path, string fileName) where T : ScriptableObject
        {
            ScriptableObject so = ScriptableObject.CreateInstance<T>();
            CreateAssetAtPath(so, path, fileName);

            return (T)so;
        }

        public static void CreateAssetAtPath(ScriptableObject so, string path, string fileName)
        {
            string[] fileNames = Directory.GetFiles(path);

            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = Path.GetFileName(fileNames[i]).Replace(Asset, string.Empty);
            }

            string unqueName = ObjectNames.GetUniqueName(fileNames, fileName);
            AssetDatabase.CreateAsset(so, path + unqueName + Asset);
            AssetDatabase.Refresh();
        }
    }
}