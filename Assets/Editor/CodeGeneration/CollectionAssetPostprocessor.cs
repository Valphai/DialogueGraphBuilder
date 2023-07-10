using Chocolate4.Dialogue.Runtime.Utilities;
using System.IO;
using UnityEditor;

namespace Chocolate4.Dialogue.Edit.CodeGeneration
{
    public class CollectionAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            foreach (string str in deletedAssets)
            {
                if (!str.EndsWith(FilePathConstants.Extension))
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(str);
                string collectionName = FilePathConstants.GetCollectionName(fileName);

                string collectionPath = FilePathConstants.GetCollectionPath(collectionName);

                if (!File.Exists(collectionPath))
                {
                    continue;
                }

                string collectionPathRelative = 
                    FilePathConstants.GetPathRelativeTo(FilePathConstants.Assets, collectionPath);

                AssetDatabase.DeleteAsset(collectionPathRelative);
            }
        }
    }
}
