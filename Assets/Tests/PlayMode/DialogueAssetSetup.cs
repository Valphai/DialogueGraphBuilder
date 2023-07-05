using B83.LogicExpressionParser;
using Chocolate4.Dialogue.Runtime;
using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Runtime.Utilities;
using Chocolate4.Runtime.Utilities.Parsing;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Dialogue.Tests.Utilities
{
    internal static class DialogueAssetSetup
    {
        private const string DialogueAsset = "dialogueAsset";
        private const string ParserAdapter = "parseAdapter";
        private const string Parser = "parser";

        internal static DialogueMaster LoadMaster()
        {
            GameObject go = new GameObject("Dialogue Master");
            DialogueMaster dialogueMaster = go.AddComponent<DialogueMaster>();

            string path =
                FilePathConstants.GetPathRelativeTo(FilePathConstants.Assets, FilePathConstants.testCasesAssetPath);

            DialogueEditorAsset asset =
                AssetDatabase.LoadAssetAtPath<DialogueEditorAsset>(path);

            FieldInfo dialogueAssetField =
                typeof(DialogueMaster).GetField(DialogueAsset, BindingFlags.NonPublic | BindingFlags.Instance);

            dialogueAssetField.SetValue(dialogueMaster, asset);

            return dialogueMaster;
        }

        internal static Parser GetParser(DialogueMaster dialogueMaster, out ParseAdapter parseAdapter)
        {
            parseAdapter = (ParseAdapter)typeof(DialogueMaster)
                            .GetField(ParserAdapter, BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(dialogueMaster);

            Parser parser = (Parser)typeof(ParseAdapter)
                .GetField(Parser, BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(parseAdapter);
            return parser;
        }
    }
}
