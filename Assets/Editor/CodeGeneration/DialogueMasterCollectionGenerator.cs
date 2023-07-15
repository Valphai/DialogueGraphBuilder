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
        public static void TryRegenerate(
            string fileName, string oldFileName,
            BlackboardSaveData blackboardSaveData, List<TreeItemSaveData> treeItemsSaveData
        )
        {
            string collectionName = FilePathConstants.GetCollectionName(fileName);
            string oldCollectionName = FilePathConstants.GetCollectionName(oldFileName);

            string oldPath = FilePathConstants.GetCollectionPath(oldCollectionName);
            if (!string.IsNullOrEmpty(oldPath) && !collectionName.Equals(oldCollectionName))
            {
                File.Delete(oldPath);
                AssetDatabase.Refresh();
            }

            string path = FilePathConstants.GetCollectionPath(collectionName);

            string pathRelativeToProjectFolder =
                FilePathConstants.GetPathRelativeTo(FilePathConstants.Assets, path);


            Writer writer = new Writer(new StringBuilder());

            writer.WriteLine("using System;");
            writer.WriteLine();
            writer.WriteLine("namespace Chocolate4.Dialogue.Runtime.Master.Collections");
            writer.BeginBlockWithIndent();
            writer.WriteLine($"public class {collectionName} : IDialogueMasterCollection");
            writer.BeginBlockWithIndent();
            writer.WriteLine($"public Type CollectionType => typeof({collectionName});");
            writer.WriteLine();

            writer.WriteLine("#region Situation Names");

            foreach (TreeItemSaveData treeItemData in treeItemsSaveData)
            {
                string displayName = treeItemData.rootItem.displayName;
                string sanitizedDisplayName = displayName.ToPascalCase().Sanitize();

                writer.WriteLine($"public const string {sanitizedDisplayName} = \"{displayName}\";");
            }

            writer.WriteLine("#endregion");
            writer.WriteLine();

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

            foreach (DialoguePropertySaveData property in eventsData)
            {
                writer.WriteLine();
                writer.WriteLine($"private void Invoke{property.displayName}() => {property.displayName}?.Invoke();");
            }

            writer.WriteLine("#endregion");

            writer.EndBlockWithIndent();
            writer.EndBlockWithIndent();

            string generatedFile = writer.buffer.ToString();
            if (FilePathConstants.FileIsDuplicate(pathRelativeToProjectFolder, generatedFile))
            {
                return;
            }

            File.WriteAllText(pathRelativeToProjectFolder, generatedFile);
            AssetDatabase.ImportAsset(pathRelativeToProjectFolder);
        }
    } 
}
