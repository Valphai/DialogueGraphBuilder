using UnityEngine;

namespace Chocolate4.Dialogue.Runtime.Utilities
{
    public static class FilePathConstants
    {
        public const string AssetFolder = "";

        public static string dialogueMasterCollectionPath = Application.dataPath + "/" + AssetFolder + "Runtime/Master/" + nameof(DialogueMasterCollection) + ".cs";
    }
}
