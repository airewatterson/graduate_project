#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MBS
{

    [Serializable]
    public class MBSAssetPack
    {
        private const string CANT_SELECT_ASSET_INDEX_LESSZERO = "MBS. AssetPack name: {0}. Can't select '{1}' asset, given @assetIndex {2} is less than 0.";
        private const string CANT_SELECT_ASSET_INDEX_BIGGER = "MBS. AssetPack name: {0}. Can't select '{1}' asset, given @assetIndex {2} is more than Assets count.";
        private const string CANT_SELECT_ASSET_MISSING = "MBS. AssetPack name: {0}. Can't select '{1}' asset at @assetIndex: {2}, reference is missing (null).";
        private const string AUTOFILL_SAME_ITEM_NUMBER = "MBS. AssetPack autofilling. Objects with the same item numbers found: {0}";


        [SerializeField, HideInInspector] public string _name;
        [SerializeField, HideInInspector] public string _guid;

        [NonSerialized] private MBSAsset[] _allAssets;
        [SerializeField] private MBSWallAsset[] _wallAssets;
        [SerializeField] private MBSFloorAsset[] _floorAssets;
        [SerializeField] public string _folderPath;
        [SerializeField] public List<string> _prefabGUIDs;

        public MBSAsset[] AllAssets
        {
            get
            {
                if (_allAssets == null)
                    RefillAllAssetsList();
                return _allAssets;
            }
        }
        public MBSWallAsset[] WallAssets
        {
            get
            {
                if (_wallAssets == null)
                    _wallAssets = new MBSWallAsset[0];
                return _wallAssets;
            }
            set
            {
                _wallAssets = value;
                RefillAllAssetsList();
            }
        }
        public MBSFloorAsset[] FloorAssets
        {
            get
            {
                if (_floorAssets == null)
                    _floorAssets = new MBSFloorAsset[0];
                return _floorAssets;
            }
            set
            {
                _floorAssets = value;
                RefillAllAssetsList();
            }
        }
        public int WallAssets_Length
        {
            get => _wallAssets.Length;
        }
        public int FloorAssets_Length
        {
            get => _floorAssets.Length;
        }



        public bool IsEmpty()
        {
            if (_wallAssets != null) _wallAssets = _wallAssets.Where(i => !i.IsEmpty()).ToArray();
            if (_floorAssets != null) _floorAssets = _floorAssets.Where(i => !i.IsEmpty()).ToArray();

            if (IsWallsEmpty() && IsFloorsEmpty())
                return true;

            return false;
        }
        public bool IsWallsEmpty()
        {
            if (_wallAssets == null || _wallAssets.Length == 0)
                return true;
            return false;
        }
        public bool IsFloorsEmpty()
        {
            if (_floorAssets == null || _floorAssets.Length == 0)
                return true;
            return false;
        }


        public bool TryGetAssetIndex(string guid, out int index)
        {
            index = -1;
            for (int i = 0; i < _allAssets.Length; i++)
            {
                if (_allAssets[i].guid == guid)
                    index = i;
            }
            if (index != -1)
                return true;
            return false;
        }


        public bool TryGetAsset(int toolNumber, int assetIndex_relativeToAll, int toolOffset, out MBSAsset asset)
        {
            asset = null;

            string assetType = ((BuilderTools)toolNumber).ToString();
            int debugIndex = assetIndex_relativeToAll - toolOffset;

            if (assetIndex_relativeToAll < 0)
            {
                Debug.LogError(string.Format(CANT_SELECT_ASSET_INDEX_LESSZERO, this._name, assetType, debugIndex));
                return false;
            }

            if (assetIndex_relativeToAll >= AllAssets.Length)
            {
                if ((AllAssets.Length - toolOffset) != 0 && assetIndex_relativeToAll != 0)
                    Debug.LogError(string.Format(CANT_SELECT_ASSET_INDEX_BIGGER, this._name, assetType, debugIndex));
                return false;
            }

            asset = AllAssets.ElementAt(assetIndex_relativeToAll);

            if (asset != null)
                return true;
            else
                Debug.LogError(string.Format(CANT_SELECT_ASSET_MISSING, this._name, assetType, debugIndex));
            return false;
        }
        public MBSAsset[] GetToolAssets(int toolIndex)
        {
            switch (toolIndex)
            {
                case (int)BuilderTools.Walls:
                    return WallAssets;
                case (int)BuilderTools.Floors:
                    return FloorAssets;
            }
            return null;
        }

        public int GetToolAssetsLength(int toolIndex)
        {
            switch (toolIndex)
            {
                case (int)BuilderTools.Walls:
                    return _wallAssets.Length;
                case (int)BuilderTools.Floors:
                    return _floorAssets.Length;
                default:
                    return 0;
            }
        }

        public void HealthCheckAndFix()
        {
            for (var i = 0; i < _wallAssets.Length; i++)
                _wallAssets[i].HealthCheckAndFix();
            for (var i = 0; i < _floorAssets.Length; i++)
                _floorAssets[i].HealthCheckAndFix();
        }

        public void FillPackWithPrefabs(string[] prefabGUIDs)
        {
            if (_wallAssets == null) _wallAssets = new MBSWallAsset[0];
            else _wallAssets = _wallAssets.Where(i => !i.IsEmpty()).ToArray();

            if (_floorAssets == null) _floorAssets = new MBSFloorAsset[0];
            else _floorAssets = _floorAssets.Where(i => !i.IsEmpty()).ToArray();

            List<GameObject> prefabs = new List<GameObject>();

            if (_prefabGUIDs != null && _prefabGUIDs.Count != 0)
            {
                _prefabGUIDs.RemoveAll(a => !prefabGUIDs.Contains(a));
                string[] intersected = _prefabGUIDs.Intersect(prefabGUIDs).ToArray();
                if (intersected.Length == _prefabGUIDs.Count && intersected.Length == prefabGUIDs.Length)
                    return;
            }
            else _prefabGUIDs = new List<string>();

            foreach (string guid in prefabGUIDs)
            {
                GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject));
                if (go != null)
                {
                    prefabs.Add(go);
                    _prefabGUIDs.Add(guid);
                }
            }

            if (prefabs.Count > 0)
            {
                SortedDictionary<int, SortedDictionary<int, GameObject>> wallDictionaries = new SortedDictionary<int, SortedDictionary<int, GameObject>>();
                SortedDictionary<int, SortedDictionary<int, GameObject>> floorDictionaries = new SortedDictionary<int, SortedDictionary<int, GameObject>>();

                Regex regexWall = new Regex(DefaultConfig.wallsNamingConvention, RegexOptions.IgnoreCase);
                Regex regexFloor = new Regex(DefaultConfig.floorsNamingConvention, RegexOptions.IgnoreCase);

                for (int i = 0; i < prefabs.Count; i++)
                {
                    GameObject prefab = prefabs[i];
                    AddToDictionaryIfNotExist(regexWall, prefab, wallDictionaries);
                    AddToDictionaryIfNotExist(regexFloor, prefab, floorDictionaries);
                }

                List<MBSWallAsset> tempWallsList = _wallAssets.ToList();
                for (int i = 0; i < wallDictionaries.Count; i++)
                {
                    SortedDictionary<int, GameObject> wallDict = wallDictionaries.ElementAt(i).Value;
                    if (_wallAssets.Length > i)
                        tempWallsList[i].FillWithFindedPrefabs(wallDict);
                    else
                        tempWallsList.Add(new MBSWallAsset(wallDict));
                }
                _wallAssets = tempWallsList.ToArray();

                List<MBSFloorAsset> tempFloorsList = _floorAssets.ToList();
                for (int i = 0; i < floorDictionaries.Count; i++)
                {
                    SortedDictionary<int, GameObject> floorDict = floorDictionaries.ElementAt(i).Value;
                    if (_floorAssets.Length > i)
                        tempFloorsList[i].FillWithFindedPrefabs(floorDict);
                    else
                        tempFloorsList.Add(new MBSFloorAsset(floorDict));
                }
                _floorAssets = tempFloorsList.ToArray();
            }

            RefillAllAssetsList();
        }

        private void RefillAllAssetsList()
        {
            List<MBSAsset> temp_all = new List<MBSAsset>();

            if (_wallAssets == null)
                _wallAssets = new MBSWallAsset[0];
            temp_all.AddRange(_wallAssets);

            if (_floorAssets == null)
                _floorAssets = new MBSFloorAsset[0];
            temp_all.AddRange(_floorAssets);

            _allAssets = temp_all.ToArray();
        }

        private void AddToDictionaryIfNotExist(Regex regex, GameObject prefab, SortedDictionary<int, SortedDictionary<int, GameObject>> assetsDict)
        {
            if (regex.IsMatch(prefab.name))
            {
                string match = regex.Match(prefab.name).Value;
                int assetNumber = int.Parse(match.Substring(1, 2));
                int prefabNumber = int.Parse(match.Substring(4, 2));

                if (assetsDict.ContainsKey(assetNumber))
                {
                    var prefabsDictionary = assetsDict[assetNumber];

                    if (!prefabsDictionary.ContainsKey(prefabNumber))
                    {
                        prefabsDictionary.Add(prefabNumber, prefab);
                    }
                    else
                        Debug.LogWarning(string.Format(AUTOFILL_SAME_ITEM_NUMBER, match));
                }
                else
                {
                    SortedDictionary<int, GameObject> prefabDict = new SortedDictionary<int, GameObject>();
                    prefabDict.Add(prefabNumber, prefab);
                    assetsDict.Add(assetNumber, prefabDict);
                }
            }
        }
    }



}
#endif
