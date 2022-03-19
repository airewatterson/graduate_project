#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace MBS
{
    public class MBSAssetsManager : ScriptableObject
    {
        public const string AllAssetPacksGUID = "AllAssetPackStaticGUID";

        private static MBSAssetsManager singleton;
        public static MBSAssetsManager Singleton
        {
            get
            {
                if (singleton != null) return singleton;
                else
                {
                    string[] guids = AssetDatabase.FindAssets("t: MBSAssetsManager", new[] { MBSConfig.Singleton.pluginCorePath });
                    if (guids.Length != 0)
                    {
                        MBSAssetsManager assetsManager = AssetDatabase.LoadAssetAtPath<MBSAssetsManager>(AssetDatabase.GUIDToAssetPath(guids[0]));
                        if (assetsManager != null)
                        {
                            singleton = assetsManager;
                        }
                    }
                    else
                    {
                        singleton = ScriptableObject.CreateInstance<MBSAssetsManager>();
                        AssetDatabase.CreateAsset(singleton, MBSConfig.Singleton.pluginCorePath + "/AssetsManager.asset");
                    }
                }
                return singleton;
            }
        }

        [SerializeField] private List<MBSAssetPack> _assetPacks;
        [NonSerialized] private string[] _assetPacksNames;

        public List<MBSAssetPack> AssetPacks
        {
            get
            {
                if (_assetPacks == null)
                    _assetPacks = new List<MBSAssetPack>();
                return _assetPacks;
            }
            set => _assetPacks = value;
        }
        public string[] AssetPacksNames
        {
            get
            {
                if (_assetPacksNames == null)
                    _assetPacksNames = GetAssetPacksNames(AssetPacks);
                return _assetPacksNames;
            }
        }


        public void RefreshAssetPacks()
        {
            _assetPacks = ParseAssetPacksFromFolder(AssetPacks);
            _assetPacksNames = GetAssetPacksNames(AssetPacks);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public List<MBSAssetPack> ParseAssetPacksFromFolder(List<MBSAssetPack> assetPacks)
        {
            MBSConfig config = MBSConfig.Singleton;
            string[] assetPacksFolders = AssetDatabase.GetSubFolders(config.pluginAssetsPath);
            if (assetPacksFolders == null || assetPacksFolders.Length == 0) return assetPacks;
            assetPacks.RemoveAll(a => !assetPacksFolders.Contains(AssetDatabase.GUIDToAssetPath(a._guid).ToString()));
            foreach (string assetPackFolder in assetPacksFolders)
            {
                if (string.IsNullOrEmpty(assetPackFolder)) continue;
                string[] splittedPath = assetPackFolder.Split(new char[] { '/' });
                if (splittedPath == null || splittedPath.Length == 0) continue;

                string assetPackName = splittedPath[splittedPath.Length - 1];
                if (string.IsNullOrEmpty(assetPackName)) continue;

                string prefabsFolder = null;
                string[] innerFolders = AssetDatabase.GetSubFolders(assetPackFolder);
                Regex regex = new Regex("/Prefabs$");
                for (int i = 0; i < innerFolders.Length; i++)
                {
                    if (regex.IsMatch(innerFolders[i]))
                    {
                        prefabsFolder = innerFolders[i];
                        break;
                    }
                }
                if (string.IsNullOrEmpty(prefabsFolder)) continue;

                string[] prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { prefabsFolder });
                if (prefabGUIDs.Length == 0) continue;

                string folderGUID = AssetDatabase.AssetPathToGUID(assetPackFolder).ToString();
                MBSAssetPack currentPack = assetPacks.SingleOrDefault(a => a._guid == folderGUID);
                if (currentPack == null)
                {
                    currentPack = new MBSAssetPack();
                    currentPack._folderPath = prefabsFolder;
                    assetPacks.Add(currentPack);
                    currentPack._name = assetPackName;
                    currentPack._guid = folderGUID;
                }
                currentPack.FillPackWithPrefabs(prefabGUIDs);
            }

            MBSAssetPack assetPack_AllIncluded = new MBSAssetPack();
            assetPack_AllIncluded._name = "--- All Asset Packs";
            assetPack_AllIncluded._guid = AllAssetPacksGUID;


            List<MBSWallAsset> tempWallsList = new List<MBSWallAsset>();
            List<MBSFloorAsset> tempFloorsList = new List<MBSFloorAsset>();

            foreach (MBSAssetPack pack in assetPacks)
            {
                if (pack.IsEmpty()) continue;

                foreach (MBSWallAsset item in pack.WallAssets)
                {
                    tempWallsList.Add(item);
                }

                foreach (MBSFloorAsset item in pack.FloorAssets)
                {
                    tempFloorsList.Add(item);
                }
            }
            assetPack_AllIncluded.WallAssets = tempWallsList.ToArray();
            assetPack_AllIncluded.FloorAssets = tempFloorsList.ToArray();
            assetPacks.Insert(0, assetPack_AllIncluded);
            return assetPacks;
        }

        public bool TryGetAssetPackIndex(string guid, out int index)
        {
            index = AssetPacks.FindIndex(a => a._guid == guid);

            if (index != -1)
                return true;
            return false;
        }

        public bool TryGetAssetPack(int index, out MBSAssetPack assetPack)
        {
            if (index < 0)
            {
                assetPack = null;
                return false;
            }

            if (AssetPacks.Count <= index)
            {
                assetPack = null;
                return false;
            }

            assetPack = AssetPacks.ElementAt(index);

            if (assetPack != null)
                return true;

            return false;
        }

        public MBSAsset GetAssetByGUID(string guid)
        {
            MBSAsset asset = null;
            for (int i = 0; i < _assetPacks.Count; i++)
            {
                asset = _assetPacks[i].AllAssets.SingleOrDefault(a => a.guid == guid);
                if (asset != null) break;
            }
            return asset;
        }

        private string[] GetAssetPacksNames(List<MBSAssetPack> assetPacks)
        {
            if (assetPacks == null || assetPacks.Count < 1)
                return null;

            string[] retval = new string[assetPacks.Count];
            for (int i = 0; i < retval.Length; i++)
            {
                if (i == 0)
                {
                    retval[i] = assetPacks[i]._name;
                    continue;
                }
                retval[i] = i + ". " + assetPacks[i]._name;
            }
            return retval;
        }







    }
}
#endif
