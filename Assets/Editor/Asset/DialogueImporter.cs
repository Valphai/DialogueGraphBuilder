using Chocolate4.Dialogue.Runtime.Asset;
using Chocolate4.Dialogue.Runtime.Utilities;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Asset
{
    [ScriptedImporter(Version, FilePathConstants.Extension)]
    public class DialogueImporter : ScriptedImporter
    {
        public const string AssetIcon = "Icons/Kok.png";
        private const int Version = 1;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            string text;
            try
            {
                text = File.ReadAllText(ctx.assetPath);
            }
            catch (Exception exception)
            {
                ctx.LogImportError($"Could not read file '{ctx.assetPath}' ({exception})");
                return;
            }

            // Create asset.
            var asset = ScriptableObject.CreateInstance<DialogueEditorAsset>();

            // Parse JSON.
            try
            {
                asset.LoadFromJson(text);
            }
            catch (Exception exception)
            {
                ctx.LogImportError($"Could not parse input actions in JSON format from '{ctx.assetPath}' ({exception})");
                DestroyImmediate(asset);
                return;
            }

            // Force name of asset to be that on the file on disk instead of what may be serialized
            // as the 'name' property in JSON.
            asset.name = Path.GetFileNameWithoutExtension(assetPath);

            var assetIcon = (Texture2D)EditorGUIUtility.Load(AssetIcon);
            
            // Add asset.
            ctx.AddObjectToAsset("<root>", asset, assetIcon);
            ctx.SetMainObject(asset);
        }
    }
}
