using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Chocolate4
{
    [ScriptedImporter(kVersion, Extension)]
    public class DialogueImporter : ScriptedImporter
    {
        private const int kVersion = 1;
        public const string Extension = "chocolatedialogue";
        private const string kAssetIcon = "Kok.png";

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

            var assetIcon = (Texture2D)EditorGUIUtility.Load(kAssetIcon);
            
            // Add asset.
            ctx.AddObjectToAsset("<root>", asset, assetIcon);
            ctx.SetMainObject(asset);

            //var maps = asset.actionMaps;

            // Create subasset for each action.
            //foreach (var map in maps)
            //{
            //    foreach (var action in map.actions)
            //    {
            //        var actionReference = ScriptableObject.CreateInstance<InputActionReference>();
            //        actionReference.Set(action);
            //        ctx.AddObjectToAsset(action.m_Id, actionReference, actionIcon);

            //        // Backwards-compatibility (added for 1.0.0-preview.7).
            //        // We used to call AddObjectToAsset using objectName instead of action.m_Id as the object name. This fed
            //        // the action name (*and* map name) into the hash generation that was used as the basis for the file ID
            //        // object the InputActionReference object. Thus, if the map and/or action name changed, the file ID would
            //        // change and existing references to the InputActionReference object would become invalid.
            //        //
            //        // What we do here is add another *hidden* InputActionReference object with the same content to the
            //        // asset. This one will use the old file ID and thus preserve backwards-compatibility. We should be able
            //        // to remove this for 2.0.
            //        //
            //        // Case: https://fogbugz.unity3d.com/f/cases/1229145/
            //        var backcompatActionReference = Instantiate(actionReference);
            //        backcompatActionReference.name = actionReference.name; // Get rid of the (Clone) suffix.
            //        backcompatActionReference.hideFlags = HideFlags.HideInHierarchy;
            //        ctx.AddObjectToAsset(actionReference.name, backcompatActionReference, actionIcon);
            //    }
            //}


            // Refresh editors.
            //Debug.Log("Refresh");
            //DialogueEditorWindow.RefreshAllOnAssetReimport();
        }
    }
}
