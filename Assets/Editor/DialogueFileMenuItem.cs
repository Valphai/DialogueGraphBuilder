using System.IO;
using System.Text;
using UnityEditor;

namespace Chocolate4.CodeGeneration
{
    public static class DialogueFileMenuItem
    {
        [MenuItem("Assets/Create/DialogueEditor")]
        public static void MakeDialogueEditorFile()
        {
            Writer writer = new Writer(new StringBuilder());

            writer.BeginBlock();

            writer.Write("\"graphSaveData\": ");
            writer.BeginBlock();
            writer.Write("\"situationSaveData\": []");
            writer.EndBlock(",");

            writer.Write("\"treeSaveData\": ");
            writer.BeginBlock();
            writer.WriteLine("\"treeItemData\": [");
            writer.BeginBlock();

            writer.Write("\"rootItem\": ");
            writer.BeginBlock();
            writer.WriteLine("\"displayName\": \"Default Situation\"");
            writer.EndBlock(",");
            writer.WriteLine("\"children\": []");
            writer.EndBlock(",");
            writer.BeginBlock();

            writer.Write("\"rootItem\": ");
            writer.BeginBlock();
            writer.WriteLine("\"displayName\": \"Default Situation\"");
            writer.EndBlock(",");
            writer.WriteLine("\"children\": []");
            writer.EndBlock(",");
            writer.BeginBlock();

            writer.Write("\"rootItem\": ");
            writer.BeginBlock();
            writer.WriteLine("\"displayName\": \"Default Situation\"");
            writer.EndBlock(",");
            writer.WriteLine("\"children\": []");
            writer.EndBlock();

            writer.WriteLine("]");
            writer.EndBlock();

            writer.EndBlock();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject) + "DialogueEditor.json";
            File.WriteAllText(path, writer.buffer.ToString());
            //AssetDatabase.ImportAsset(path);
        }
    }
}
