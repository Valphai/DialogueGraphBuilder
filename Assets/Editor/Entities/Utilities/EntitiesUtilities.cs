using Chocolate4.Dialogue.Runtime.Saving;
using UnityEditor;
using UnityEngine;

namespace Chocolate4.Edit.Entities.Utilities
{
    public static class EntitiesUtilities
    {
        public static Texture2D GetEntityImage(DialogueEntity entity)
        {
            return entity.entityImage == null 
                ? AssetPreview.GetMiniTypeThumbnail(typeof(Sprite)) 
                : AssetPreview.GetAssetPreview(entity.entityImage);
        }
    }
}