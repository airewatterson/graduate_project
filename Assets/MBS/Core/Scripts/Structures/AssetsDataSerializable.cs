#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace MBS
{
    [Serializable]
    public class AssetsDataSerializable : ScriptableObject
    {
        [SerializeField] public BuilderTools currentTool;
        [SerializeField] public Vector2[] scrollPos;
        [SerializeField] public float gridSize;
        [SerializeField] public Vector3 gridPosition;
        [SerializeField] public float levelHeight;
        [SerializeField] public int levelNumber;

        [SerializeField] public string[] assetPackGUIDs;
        [SerializeField] public string[] assetGUIDs;

        private void OnEnable()
        {
            name = "AssetsDataSerializable";
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            EditorUtility.SetDirty(this);
        }
    }
}
#endif
