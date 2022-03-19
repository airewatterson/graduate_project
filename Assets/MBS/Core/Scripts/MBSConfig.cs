#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace MBS
{
    [System.Serializable]
    public class MBSConfig : ScriptableObject, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string MBS_PLUGIN_FOLDER_NOT_FOUND = "MBS. Fatal Error. Can't find MBS Plugin folder, can not setup config file and initialize plugin." +
                                                           " If you see this, then probably you have to rename MBS plugin folder back to MBS.";
        private static MBSConfig singleton;

        [SerializeField] public string pluginPath;
        [SerializeField] public string pluginAssetsPath;
        [SerializeField] public string pluginCorePath;
        [SerializeField] public string pluginInternalPath;
        [SerializeField] public List<(Texture2D, Texture2D)> toolbarIcons;
        [SerializeField] public List<(Texture2D, Texture2D)> heightPickerIcons;
        [SerializeField] public List<(Texture2D, Texture2D)> gridSizePickerIcons;
        [SerializeField] public Texture2D[,] toolbarIconsBW;
        [SerializeField] public Mesh wallGizmoMesh;
        [SerializeField] public Mesh gridMesh;
        [SerializeField] public Material gridMaterial;
        [SerializeField] public bool pluginDisabled = false;


        public static MBSConfig Singleton
        {
            get
            {
                if (singleton != null)
                {
                    singleton.HealthCheckAndFix();
                    return singleton;
                }
                else
                {
                    string mbsPath = MBSHelper.SearchFolderInAssets("MBS");
                    if (string.IsNullOrEmpty(mbsPath))
                    {
                        Debug.LogError(MBS_PLUGIN_FOLDER_NOT_FOUND);
                        return null;
                    }

                    string corePath = mbsPath + DefaultConfig.pluginCorePath;

                    string[] guids = AssetDatabase.FindAssets("t: MBSConfig", new[] { corePath });
                    if (guids.Length != 0)
                    {
                        MBSConfig conf = AssetDatabase.LoadAssetAtPath<MBSConfig>(AssetDatabase.GUIDToAssetPath(guids[0]));
                        if (conf != null)
                        {
                            singleton = conf;
                            singleton.Initialize();
                        }
                    }
                    else
                    {
                        singleton = ScriptableObject.CreateInstance<MBSConfig>();
                        singleton.Initialize(mbsPath);
                        AssetDatabase.CreateAsset(singleton, corePath + "/MBSConfig.asset");
                    }
                }
                return singleton;
            }
        }

        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            Singleton.pluginDisabled = true;
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            Singleton.pluginDisabled = false;
        }


        private void OnEnable()
        {
            name = "MBS Config";

            if (singleton != null)
            {
                if (singleton != this)
                    DestroyImmediate(this);
            }
            else singleton = this;
        }


        public void Initialize(string mbsPluginPath = null)
        {
            if (string.IsNullOrEmpty(mbsPluginPath))
            {
                mbsPluginPath = MBSHelper.SearchFolderInAssets("MBS");

                if (string.IsNullOrEmpty(mbsPluginPath))
                {
                    Debug.LogError(MBS_PLUGIN_FOLDER_NOT_FOUND);
                    return;
                }
            }

            pluginPath = mbsPluginPath;
            pluginAssetsPath = pluginPath + DefaultConfig.pluginAssetsPath;
            pluginCorePath = pluginPath + DefaultConfig.pluginCorePath;
            pluginInternalPath = pluginPath + DefaultConfig.pluginInternalPath;

            gridMesh = (Mesh)AssetDatabase.LoadAssetAtPath(pluginInternalPath + DefaultConfig.gridPlaneMesh, typeof(Mesh));
            gridMaterial = (Material)AssetDatabase.LoadAssetAtPath(pluginInternalPath + DefaultConfig.gridMat, typeof(Material));
            wallGizmoMesh = (Mesh)AssetDatabase.LoadAssetAtPath(pluginInternalPath + DefaultConfig.gizmoMesh, typeof(Mesh));

            if (toolbarIcons == null || toolbarIcons.Count == 0)
            {
                toolbarIcons = new List<(Texture2D, Texture2D)>();
                for (int i = 0; i < DefaultConfig.toolbarIcons.GetLength(0); i++)
                {
                    toolbarIcons.Add((LoadTextureFromPath(pluginInternalPath + DefaultConfig.toolbarIcons[i, 0]),
                                      LoadTextureFromPath(pluginInternalPath + DefaultConfig.toolbarIcons[i, 1])));
                }
            }

            if (heightPickerIcons == null || heightPickerIcons.Count != DefaultConfig.heightPickerIcons.GetLength(0))
            {
                heightPickerIcons = new List<(Texture2D, Texture2D)>();
                for (int i = 0; i < DefaultConfig.heightPickerIcons.GetLength(0); i++)
                {
                    heightPickerIcons.Add((LoadTextureFromPath(pluginInternalPath + DefaultConfig.heightPickerIcons[i, 0]),
                                           LoadTextureFromPath(pluginInternalPath + DefaultConfig.heightPickerIcons[i, 1])));
                }
            }

            if (gridSizePickerIcons == null || gridSizePickerIcons.Count != DefaultConfig.gridSizePickerIcons.GetLength(0))
            {
                gridSizePickerIcons = new List<(Texture2D, Texture2D)>();
                for (int i = 0; i < DefaultConfig.gridSizePickerIcons.GetLength(0); i++)
                {
                    gridSizePickerIcons.Add((LoadTextureFromPath(pluginInternalPath + DefaultConfig.gridSizePickerIcons[i, 0]),
                                           LoadTextureFromPath(pluginInternalPath + DefaultConfig.gridSizePickerIcons[i, 1])));
                }
            }

        }


        public void HealthCheckAndFix()
        {
            if (string.IsNullOrEmpty(pluginPath) || string.IsNullOrEmpty(pluginAssetsPath) ||
                string.IsNullOrEmpty(pluginCorePath) || string.IsNullOrEmpty(pluginInternalPath) ||
                gridMesh == null || gridMaterial == null || wallGizmoMesh == null ||
                toolbarIcons == null || toolbarIcons.Count != DefaultConfig.toolbarIcons.GetLength(0) ||
                heightPickerIcons == null || heightPickerIcons.Count != DefaultConfig.heightPickerIcons.GetLength(0))
                Initialize();
        }

        private Texture2D LoadTextureFromPath(string fullPath)
        {
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
            if (tex != null) return tex;
            else
            {
                return null;
            }
        }

        int a;


    }

    public static class DefaultConfig
    {
        public const string TAG_WALL = "MBS_WALL";
        public const string TAG_FLOOR = "MBS_FLOOR";

        public static string wallsNamingConvention = @"w\d{2}-\d{2}";
        public static string floorsNamingConvention = @"f\d{2}-\d{2}";

        public static string pluginCorePath = "/Core";
        public static string pluginAssetsPath = "/AssetPacks";
        public static string pluginInternalPath = "/InternalDataStore";

        public static string gizmoMesh = "/Meshes/MBSGizmo.fbx";
        public static string gridPlaneMesh = "/Meshes/MBSGridPLane.fbx";
        public static string gridMat = "/Materials/MBSGridMaterial.mat";

        public static string[,] toolbarIcons = {
            { "/Icons/iconToolbarWall-w.png", "/Icons/iconToolbarWall-b.png" },
            { "/Icons/iconToolbarFloor-w.png", "/Icons/iconToolbarFloor-b.png" }
        };

        public static string[,] heightPickerIcons = {
            { "/Icons/iconPosPicker-w.png", "/Icons/iconPosPicker-b.png" },
            { "/Icons/iconObjPicker-w.png", "/Icons/iconObjPicker-b.png" }
        };

        public static string[] heightPickerNames = {
            "Height picker",
            "Object picker"
        };

        public static string[,] gridSizePickerIcons = {
            { "/Icons/iconGridSizeLink-w.png", "/Icons/iconGridSizeLink-b.png" },
            { "/Icons/iconGridSizePicker-w.png", "/Icons/iconGridSizePicker-b.png" }
        };

        public static string[] gridSizePickerNames = {
            "Object link",
            "Object picker"
        };
    }
}
#endif


