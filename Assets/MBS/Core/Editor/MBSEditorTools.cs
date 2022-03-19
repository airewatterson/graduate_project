using UnityEngine;
using UnityEditor;

namespace MBS
{
    public class MBSEditorTools
    {
        public static Texture2D GetAssetPreviewOrGray(GameObject gameObject)
        {
            if (gameObject == null)
                return Texture2D.grayTexture;

            Texture2D assetPreview = AssetPreview.GetAssetPreview(gameObject);

            if (assetPreview == null)
                assetPreview = Texture2D.grayTexture;

            return assetPreview;
        }
    }
}
