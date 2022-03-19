#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace MBS
{
    public class AssetsData
    {
        private const string CANT_FIND_ASSET_BY_GUID = "MBS. Asset Data. Can't find asset within last saved GUID. Asset can be removed or GUID can be changed.";
        private const string ASSET_PACK_NULL_SELECT_ASSET = "MBS. AssetData. Can't select asset because AssetPack is missing.";
        private const string SERIALIZABLE_NULL = "MBS. Can not initialize AssetData from serializable object because it is null.";



        [NonSerialized] public MBSBuilder _builder;

        [NonSerialized] private GUIContent[] _toolbarContent;
        [NonSerialized] private GUIContent[] _heightPickerContent;
        [NonSerialized] private GUIContent[] _gridSizePickerContent;
        [SerializeField] private BuilderTools _current_Tool;
        [SerializeField] private Vector2[] _scrollPos;
        [SerializeField] private float _gridSize;
        [SerializeField] private Vector3 _gridPosition;
        [SerializeField] private float _levelHeight;
        [SerializeField] private int _levelNumber;


        [SerializeField] private string[] _assetPacksGUIDs;
        [SerializeField] private string[] _assetsGUIDs;
        [NonSerialized] private MBSAssetPack[] _assetPacks;
        [NonSerialized] private MBSAsset[] _assets;
        [NonSerialized] private Vector3[] _assetSizes;
        [NonSerialized] private MeshFilter[,] _meshFilters;
        [NonSerialized] private GameObject[,] _firstPrefabs;
        [NonSerialized] private PrefabType[,] _firstPrefabTypes;
        [NonSerialized] private int[] _assetPacksIndexes;
        [NonSerialized] private int[] _assetsIndexes;
        [NonSerialized] private int _toolAssetsOffset;

        [NonSerialized] public static int _toolsLength = Enum.GetNames(typeof(BuilderTools)).Length;

        public string[] AssetPacksGUIDs
        {
            get => _assetPacksGUIDs;
        }
        public string[] AssetsGUIDs
        {
            get => _assetsGUIDs;
        }

        public BuilderTools Current_Tool
        {
            get => _current_Tool;
            set => _current_Tool = value;
        }
        public int Current_Tool_Int
        {
            get => (int)_current_Tool;
        }
        public MBSAssetPack Current_AssetPack
        {
            get => _assetPacks[Current_Tool_Int];
        }
        public MBSAsset Current_Asset
        {
            get => _assets[Current_Tool_Int];
        }
        public Vector3 Current_Asset_Size
        {
            get => _assetSizes[Current_Tool_Int];
        }
        public MeshFilter Current_Asset_MeshFilter(FloorTileType floorType = FloorTileType.None)
        {
            if (_current_Tool == BuilderTools.Floors)
            {
                if (floorType != FloorTileType.None)
                {
                    return _meshFilters[Current_Tool_Int, (int)floorType];
                }
            }
            return _meshFilters[Current_Tool_Int, 0];
        }
        public GameObject Current_Asset_FirstPrefab(FloorTileType floorType = FloorTileType.None)
        {
            if (floorType == FloorTileType.Corner)
                return _firstPrefabs[Current_Tool_Int, 1];

            return _firstPrefabs[Current_Tool_Int, 0];
        }
        public PrefabType Current_Asset_FirstPrefab_Type(FloorTileType floorType = FloorTileType.None)
        {
            if (floorType == FloorTileType.Corner)
                return _firstPrefabTypes[Current_Tool_Int, 1];

            return _firstPrefabTypes[Current_Tool_Int, 0];
        }

        public int Current_AssetPack_Index
        {
            get => _assetPacksIndexes[Current_Tool_Int];
        }
        public int Current_Asset_Index
        {
            get => _assetsIndexes[Current_Tool_Int] - GetToolAssetsOffset(Current_Tool_Int);
        }

        public MBSAsset[] Current_AssetPack_Assets()
        {
            return Current_AssetPack?.GetToolAssets(Current_Tool_Int);
        }
        public int Current_AssetPack_AssetsLength()
        {
            if (Current_AssetPack != null)
                return Current_AssetPack.GetToolAssetsLength(Current_Tool_Int);
            else return 0;
        }


        public int LevelNumber
        {
            get => _levelNumber;
            set => _levelNumber = value;
        }
        public float LevelHeight
        {
            get => _levelHeight;
            set => _levelHeight = value;
        }
        public float CalcLevelHeight
        {
            get => LevelNumber * LevelHeight;
        }


        public float GridSize
        {
            get => _gridSize;
            set => _gridSize = Mathf.Clamp(value, 0.1f, 100);
        }
        public Vector3 GridPosition
        {
            get => _gridPosition;
            set => _gridPosition = value;
        }
        public Vector2 ScrollPos
        {
            get => _scrollPos[Current_Tool_Int];
            set => _scrollPos[Current_Tool_Int] = value;
        }
        public GUIContent[] ToolbarContent
        {
            get => _toolbarContent;
        }
        public GUIContent[] HeightPickerContent
        {
            get => _heightPickerContent;
        }
        public GUIContent[] GridSizePickerContent
        {
            get => _gridSizePickerContent;
        }

        public int GetToolAssetsOffset(int toolAsset)
        {
            switch (toolAsset)
            {
                case 0:
                    return 0;
                case 1:
                    return Current_AssetPack.WallAssets.Length;
            }
            return 0;
        }

        public AssetsData(MBSBuilder builder)
        {
            this._builder = builder;
            Initialize();
        }
        public AssetsData(MBSBuilder builder, AssetsDataSerializable serializable)
        {
            if (serializable == null)
            {
                Debug.LogError(SERIALIZABLE_NULL);
                return;
            }

            this._builder = builder;

            this._current_Tool = serializable.currentTool;
            this._gridSize = serializable.gridSize;
            this._scrollPos = serializable.scrollPos;
            this._levelHeight = serializable.levelHeight;
            this._levelNumber = serializable.levelNumber;

            this._assetPacksGUIDs = serializable.assetPackGUIDs;
            this._assetsGUIDs = serializable.assetGUIDs;
        }
        public AssetsDataSerializable GetSerializable(AssetsDataSerializable serializable)
        {
            if (serializable == null)
            {
                serializable = ScriptableObject.CreateInstance<AssetsDataSerializable>();

                serializable.assetPackGUIDs = this.AssetPacksGUIDs;
                serializable.assetGUIDs = this.AssetsGUIDs;
                serializable.currentTool = this.Current_Tool;
                serializable.gridSize = this._gridSize;
                serializable.gridPosition = this._gridPosition;
                serializable.scrollPos = this._scrollPos;
                serializable.levelHeight = this._levelHeight;
                serializable.levelNumber = this._levelNumber;
                EditorUtility.SetDirty(serializable);
                EditorUtility.SetDirty(_builder);
            }
            else
            {
                bool isSmthChanged = false;

                if (this.AssetPacksGUIDs == null || this.AssetPacksGUIDs.Length != _toolsLength)
                {
                    Initialize();
                }

                if (!Enumerable.SequenceEqual(serializable.assetPackGUIDs, this.AssetPacksGUIDs))
                { serializable.assetPackGUIDs = this.AssetPacksGUIDs; isSmthChanged = true; }

                if (!Enumerable.SequenceEqual(serializable.assetGUIDs, this.AssetsGUIDs))
                { serializable.assetGUIDs = this.AssetsGUIDs; isSmthChanged = true; }

                if (serializable.currentTool != this.Current_Tool)
                { serializable.currentTool = this.Current_Tool; isSmthChanged = true; }

                if (serializable.levelHeight != this.LevelHeight)
                { serializable.levelHeight = this.LevelHeight; isSmthChanged = true; }

                if (serializable.levelNumber != this.LevelNumber)
                { serializable.levelNumber = this.LevelNumber; isSmthChanged = true; }

                if (serializable.gridSize != this._gridSize)
                { serializable.gridSize = this._gridSize; isSmthChanged = true; }

                if (serializable.gridPosition != this._gridPosition)
                { serializable.gridPosition = this._gridPosition; isSmthChanged = true; }

                if (!Enumerable.SequenceEqual(serializable.scrollPos, this._scrollPos))
                { serializable.scrollPos = this._scrollPos; isSmthChanged = true; }

                if (isSmthChanged)
                {
                    EditorUtility.SetDirty(serializable);
                    EditorUtility.SetDirty(_builder);
                }
            }
            return serializable;
        }

        public void Initialize()
        {
            if (_gridSize < 0.1f) _gridSize = 1;
            if (_scrollPos == null || _scrollPos.Length != _toolsLength) _scrollPos = new Vector2[_toolsLength];
            if (_toolbarContent == null || _toolbarContent.Length != _toolsLength) _toolbarContent = GetToolbarGUIContent();
            if (_heightPickerContent == null || _heightPickerContent.Length != 2) _heightPickerContent = GetHeightPickerGUIContent();
            if (_gridSizePickerContent == null || _gridSizePickerContent.Length != 1) _gridSizePickerContent = GetGridSizePickerGUIContent();


            if (_assetPacks == null || _assetPacks.Length != _toolsLength) _assetPacks = new MBSAssetPack[_toolsLength];
            if (_assetPacksIndexes == null || _assetPacksIndexes.Length != _toolsLength) _assetPacksIndexes = new int[_toolsLength];

            if (_assets == null || _assets.Length != _toolsLength) _assets = new MBSAsset[_toolsLength];
            if (_assetsIndexes == null || _assetsIndexes.Length != _toolsLength) _assetsIndexes = new int[_toolsLength];
            if (_assetSizes == null || _assetSizes.Length != _toolsLength) _assetSizes = new Vector3[_toolsLength];
            if (_meshFilters == null || _meshFilters.GetLength(0) != _toolsLength) _meshFilters = new MeshFilter[_toolsLength, 2];
            if (_firstPrefabs == null || _firstPrefabs.GetLength(0) != _toolsLength) _firstPrefabs = new GameObject[_toolsLength, 2];
            if (_firstPrefabTypes == null || _firstPrefabTypes.GetLength(0) != _toolsLength) _firstPrefabTypes = new PrefabType[_toolsLength, 2];


            if (_assetPacksIndexes == null || _assetPacksIndexes.Length != _toolsLength) _assetPacksIndexes = new int[_toolsLength];
            if (_assetsIndexes == null || _assetsIndexes.Length != _toolsLength) _assetsIndexes = new int[_toolsLength];

            if (_assetPacksGUIDs == null || _assetPacksGUIDs.Length != _toolsLength || _assetPacksGUIDs.Any(i => string.IsNullOrEmpty(i)))
            {
                _assetPacksGUIDs = new string[_toolsLength];
                _assetsGUIDs = new string[_toolsLength];
                Try_SetupCurrentAssetPack(0, 0);
                Try_SetupCurrentAssetPack(1, 0);
            }
            else SetupAssetPacksByGUID();

            if (_assetsGUIDs == null || _assetsGUIDs.Length != _toolsLength || _assetsGUIDs.Any(i => string.IsNullOrEmpty(i)))
            {
                _assetsGUIDs = new string[_toolsLength];
                Try_SetupCurrentAsset(0, 0);
                Try_SetupCurrentAsset(1, 0);
            }
            else SetupAllToolAssetsByGUID();
        }

        public void SetupAssetPacksByGUID()
        {
            for (int i = 0; i < _toolsLength; i++)
            {
                if (AssetPacksGUIDs[i] != MBSAssetsManager.AllAssetPacksGUID)
                {
                    if (MBSAssetsManager.Singleton.TryGetAssetPackIndex(AssetPacksGUIDs[i], out int packIndex))
                    {
                        if (MBSAssetsManager.Singleton.TryGetAssetPack(packIndex, out MBSAssetPack assetPack))
                        {
                            _assetPacks[i] = assetPack;
                            _assetPacksIndexes[i] = packIndex;
                            _assetPacksGUIDs[i] = assetPack._guid;
                            continue;
                        }
                    }
                }

                _assetPacks[i] = MBSAssetsManager.Singleton.AssetPacks.ElementAtOrDefault(0);
                _assetPacksIndexes[i] = 0;
                _assetPacksGUIDs[i] = _assetPacks[i]._guid;
            }
        }
        public void SetupAllToolAssetsByGUID()
        {
            for (int i = 0; i < _toolsLength; i++)
            {
                if (_assetPacks[i] != null)
                {
                    if (!string.IsNullOrEmpty(AssetsGUIDs[i]))
                    {
                        if (_assetPacks[i].TryGetAssetIndex(_assetsGUIDs[i], out int assetIndex))
                        {
                            if (Try_SetupCurrentAsset(i, assetIndex - GetToolAssetsOffset(i)))
                                continue;
                        }
                    }

                    if (!Try_SetupCurrentAsset(i, 0))
                    {
                        Debug.LogError(CANT_FIND_ASSET_BY_GUID);
                    }
                }
            }
        }

        public bool Try_SetupCurrentAssetPack(int toolNumber, int itemIndex)
        {
            if (MBSAssetsManager.Singleton.TryGetAssetPack(itemIndex, out MBSAssetPack assetPack))
            {
                assetPack.HealthCheckAndFix();

                _assetPacks[toolNumber] = assetPack;
                _assetPacksIndexes[toolNumber] = itemIndex;
                _assetPacksGUIDs[toolNumber] = assetPack._guid;
                return true;
            }

            _assetPacks[toolNumber] = null;
            _assetPacksIndexes[toolNumber] = 0;
            _assetPacksGUIDs[toolNumber] = null;
            return false;
        }
        public bool Try_SetupCurrentAsset(int toolNumber, int itemIndex)
        {
            if (_assetPacks[toolNumber] == null)
            {
                Debug.LogError(ASSET_PACK_NULL_SELECT_ASSET);
                return false;
            }


            int indexWithOffset = itemIndex + GetToolAssetsOffset(toolNumber);



            if (_assetPacks[toolNumber].TryGetAsset(toolNumber, indexWithOffset, GetToolAssetsOffset(toolNumber), out MBSAsset findedAsset))
            {
                if (findedAsset.TryGetPrefab(0, out GameObject prefab, out PrefabType prefabType))
                {
                    _firstPrefabs[toolNumber, 0] = prefab;
                    _firstPrefabTypes[toolNumber, 0] = prefabType;

                    MeshFilter meshFilter = null;
                    MeshFilter meshFilter1 = null;

                    switch (prefabType)
                    {
                        case PrefabType.Basic:
                            meshFilter = prefab.GetComponent<MeshFilter>();

                            if (toolNumber == (int)BuilderTools.Floors)
                            {
                                if (findedAsset.TryGetPrefab(1, out GameObject cornerPrefab1, out PrefabType cornerType1, false))
                                {
                                    meshFilter1 = cornerPrefab1.GetComponent<MeshFilter>();
                                    _firstPrefabs[toolNumber, 1] = cornerPrefab1;
                                    _firstPrefabTypes[toolNumber, 1] = cornerType1;
                                }
                            }
                            break;

                        case PrefabType.LodGroup:
                            foreach (Transform child in prefab.transform)
                            {
                                if (child.TryGetComponent<MeshFilter>(out MeshFilter mFilter))
                                {
                                    if (mFilter.sharedMesh != null)
                                    {
                                        meshFilter = mFilter;
                                        break;
                                    }
                                }
                            }

                            if (toolNumber == (int)BuilderTools.Floors)
                            {
                                if (findedAsset.TryGetPrefab(1, out GameObject cornerPrefab2, out PrefabType cornerType2))
                                {
                                    _firstPrefabs[toolNumber, 1] = cornerPrefab2;
                                    _firstPrefabTypes[toolNumber, 1] = cornerType2;

                                    foreach (Transform child in cornerPrefab2.transform)
                                    {
                                        if (child.TryGetComponent<MeshFilter>(out MeshFilter cornerMFilter))
                                        {
                                            if (cornerMFilter.sharedMesh != null)
                                            {
                                                meshFilter1 = cornerMFilter;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    _assets[toolNumber] = findedAsset;
                    _assetsIndexes[toolNumber] = indexWithOffset;
                    _assetsGUIDs[toolNumber] = findedAsset.guid;
                    _meshFilters[toolNumber, 0] = meshFilter;
                    _meshFilters[toolNumber, 1] = meshFilter1;
                    _assetSizes[toolNumber] = new Vector3(
                        meshFilter.sharedMesh.bounds.size.x * meshFilter.transform.localScale.x,
                        meshFilter.sharedMesh.bounds.size.y * meshFilter.transform.localScale.y,
                        meshFilter.sharedMesh.bounds.size.z * meshFilter.transform.localScale.z
                    ).RoundDecimals();
                    return true;
                }

            }

            _assets[toolNumber] = null;
            _assetsIndexes[toolNumber] = -1;
            _assetsGUIDs[toolNumber] = null;
            _meshFilters[toolNumber, 0] = null;
            _meshFilters[toolNumber, 1] = null;
            _firstPrefabs[toolNumber, 0] = null;
            _firstPrefabs[toolNumber, 1] = null;
            _assetSizes[toolNumber] = default;

            return false;
        }

        public GUIContent[] GetToolbarGUIContent()
        {
            GUIContent[] retval = new GUIContent[System.Enum.GetNames(typeof(BuilderTools)).Length];
            int skin = (EditorGUIUtility.isProSkin) ? 0 : 1;

            for (int i = 0; i < retval.Length; i++)
            {
                if (skin == 0)
                {
                    if (MBSConfig.Singleton.toolbarIcons.Count > i)
                        retval[i] = EditorGUIUtility.TrIconContent(MBSConfig.Singleton.toolbarIcons[i].Item1, System.Enum.GetName(typeof(BuilderTools), i));
                    else retval[i] = null;
                }
                else
                {
                    if (MBSConfig.Singleton.toolbarIcons.Count > i)
                        retval[i] = EditorGUIUtility.TrIconContent(MBSConfig.Singleton.toolbarIcons[i].Item2, System.Enum.GetName(typeof(BuilderTools), i));
                    else retval[i] = null;
                }
            }
            return retval;
        }
        public GUIContent[] GetHeightPickerGUIContent()
        {
            GUIContent[] retval = new GUIContent[2];
            int skin = (EditorGUIUtility.isProSkin) ? 0 : 1;

            for (int i = 0; i < retval.Length; i++)
            {
                if (skin == 0)
                {
                    if (MBSConfig.Singleton.heightPickerIcons.Count > i)
                        retval[i] = EditorGUIUtility.TrIconContent(MBSConfig.Singleton.heightPickerIcons[i].Item1, DefaultConfig.heightPickerNames[i]);
                    else retval[i] = null;
                }
                else
                {
                    if (MBSConfig.Singleton.heightPickerIcons.Count > i)
                        retval[i] = EditorGUIUtility.TrIconContent(MBSConfig.Singleton.heightPickerIcons[i].Item2, DefaultConfig.heightPickerNames[i]);
                    else retval[i] = null;
                }
            }
            return retval;
        }
        public GUIContent[] GetGridSizePickerGUIContent()
        {
            GUIContent[] retval = new GUIContent[DefaultConfig.gridSizePickerIcons.GetLength(0)];
            int skin = (EditorGUIUtility.isProSkin) ? 0 : 1;

            for (int i = 0; i < retval.Length; i++)
            {
                if (skin == 0)
                {
                    if (MBSConfig.Singleton.gridSizePickerIcons.Count > i)
                        retval[i] = EditorGUIUtility.TrIconContent(MBSConfig.Singleton.gridSizePickerIcons[i].Item1, DefaultConfig.gridSizePickerNames[i]);
                    else retval[i] = null;
                }
                else
                {
                    if (MBSConfig.Singleton.gridSizePickerIcons.Count > i)
                        retval[i] = EditorGUIUtility.TrIconContent(MBSConfig.Singleton.gridSizePickerIcons[i].Item2, DefaultConfig.gridSizePickerNames[i]);
                    else retval[i] = null;
                }
            }
            return retval;
        }

        public void SetGridSize_Wall()
        {
            GridSize = _assetSizes[(int)BuilderTools.Walls].x;
        }
        public void ResetGridSize_Wall()
        {
            GridSize = 1;
        }
    }
}
#endif