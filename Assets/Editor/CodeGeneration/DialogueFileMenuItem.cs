using Chocolate4.Dialogue.Edit.Asset;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.CodeGeneration
{
    public static class DialogueFileMenuItem
    {
        private const string FileName = "DialogueEditor";

        [MenuItem("Assets/Create/DialogueEditor")]
        public static void MakeDialogueEditorFile()
        {
            Writer writer = WriteDefaultContent();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject) + FilePathConstants.DirSep + FileName + "." + FilePathConstants.Extension;

            ProjectWindowUtil.CreateAssetWithContent(path, writer.buffer.ToString(), (Texture2D)EditorGUIUtility.Load(DialogueImporter.AssetIcon));

            AssetDatabase.Refresh();
        }

        public static Writer WriteDefaultContent()
        {
            Writer writer = new Writer(new StringBuilder());

            writer.BeginBlock();

            writer.Write("\"graphSaveData\": ");
            writer.BeginBlock();
            writer.WriteLine("\"situationSaveData\": []");
            writer.EndBlock(",");

            writer.Write("\"treeSaveData\": ");
            writer.BeginBlock();
            writer.WriteLine("\"treeItemData\": []");
            writer.EndBlock();

            writer.EndBlock();
            return writer;
        }
    }
}
