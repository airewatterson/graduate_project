#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MBS
{
    public static class MBSHelper
    {
        private const string INPUT_STRING_NULL_OR_EMPTY = "MBS. Input string @searchFolder can't be null or empty.";

        [MenuItem("Tools/MBS/Create MBSBuilder", false, 0)]
        public static void CreateMBSBuilder()
        {
            GameObject g = new GameObject();
            g.transform.position = Vector3.zero;
            g.name = "MBS Builder";
            g.AddComponent<MBSBuilder>();
            Selection.objects = new[] { g };
            g.RecordCreatedUndo("Create MBSBuilder");
        }

        [MenuItem("Tools/MBS/Enable All Components", false, 11)]
        public static void EnableAllComponents()
        {
            MBSConfig.Singleton.pluginDisabled = false;
        }

        [MenuItem("Tools/MBS/Disable All Components", false, 12)]
        public static void DisableAllComponents()
        {
            MBSConfig.Singleton.pluginDisabled = true;
        }

        public static string SearchFolderInAssets(string searchFolder)
        {
            if (string.IsNullOrEmpty(searchFolder))
            {
                Debug.LogError(INPUT_STRING_NULL_OR_EMPTY);
                return null;
            }

            string pathToFolder = null;
            var folders = AssetDatabase.GetSubFolders("Assets");
            Regex regEx = new Regex(@"/" + searchFolder + "$");

            foreach (var folder in folders)
            {
                pathToFolder = SearchFolderRecursive(folder, regEx);
                if (pathToFolder != null)
                    return pathToFolder;
            }

            return pathToFolder;
        }
        private static string SearchFolderRecursive(string inputFolder, Regex regex)
        {
            if (regex.IsMatch(inputFolder))
            {
                return inputFolder;
            }

            string path = null;

            var folders = AssetDatabase.GetSubFolders(inputFolder);
            foreach (var fld in folders)
            {
                path = SearchFolderRecursive(fld, regex);
                if (path != null)
                    return path;
            }
            return null;
        }
    }
}
#endif