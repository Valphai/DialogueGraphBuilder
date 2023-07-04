using Chocolate4.Dialogue.Runtime.Saving;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Chocolate4.Dialogue.Edit.CodeGeneration
{
    public static class DialogueMasterCollectionGenerator
	{
        public static void TryRegenerate(BlackboardSaveData blackboardSaveData)
		{
            string path = FilePathConstants.dialogueMasterCollectionPath;

            int lastIndex = path.LastIndexOf("Assets");
            string pathRelativeToProjectFolder = FilePathConstants.dialogueMasterCollectionPath.Substring(lastIndex);

            Writer writer = new Writer(new StringBuilder());

            writer.WriteLine("using System;");
            writer.WriteLine();
            writer.WriteLine("namespace Chocolate4.Dialogue.Runtime");
            writer.BeginBlockWithIndent();
            writer.WriteLine("public class DialogueMasterCollection");
            writer.BeginBlockWithIndent();

            writer.WriteLine("#region Variables");

            List<DialoguePropertySaveData> dialoguePropertiesSaveData = 
                blackboardSaveData.dialoguePropertiesSaveData;


            List<DialoguePropertySaveData> valuesData = 
                dialoguePropertiesSaveData.Where(prop => prop.propertyType != PropertyType.Event).ToList();
            foreach (DialoguePropertySaveData property in valuesData)
            {
                writer.WriteLine($"public {property.propertyType.GetPropertyString()} {property.displayName} {{ get; set; }} = {property.value.ToLower()};");
            }

            writer.WriteLine("#endregion");
            writer.WriteLine();
            writer.WriteLine("#region Events");

            List<DialoguePropertySaveData> eventsData = dialoguePropertiesSaveData.Except(valuesData).ToList();
            foreach (DialoguePropertySaveData property in eventsData)
            {
                writer.WriteLine($"public {property.propertyType.GetPropertyString()} {property.displayName};");
            }

            writer.WriteLine("#endregion");

            writer.EndBlockWithIndent();
            writer.EndBlockWithIndent();

            string existingFile = File.ReadAllText(path);
            string generatedFile = writer.buffer.ToString();

            if (existingFile == generatedFile)
            {
                return;
            }

            File.WriteAllText(path, generatedFile);
            AssetDatabase.ImportAsset(pathRelativeToProjectFolder);
        }
	} 
}
