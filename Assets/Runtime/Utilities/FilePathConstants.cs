using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Utilities
{
    public static class FilePathConstants
    {
        public const string Assets = "Assets";
        public const string AssetFolder = "";

        public static string dialogueMasterCollectionPath = Application.dataPath + "/" + AssetFolder + "Runtime/Master/" + nameof(DialogueMasterCollection) + ".cs";
        public static string testCasesAssetPath = Application.dataPath + "/" + "Tests/PlayMode/Cases/TestCasesDialogueEditor.chocolatedialogue";

        public static string GetPathRelativeTo(string start, string path)
        {
            int lastIndex = path.LastIndexOf(start);
            return path.Substring(lastIndex);
        }
    }
}
