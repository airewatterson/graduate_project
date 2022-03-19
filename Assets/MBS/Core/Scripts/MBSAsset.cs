#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MBS
{
    [Serializable]
    public class MBSAsset
    {
        private const string CANT_SELECT_PREFAB_EMPTY = "MBS. Asset name: {0}. Can't select prefab at @prefabIndex: {1}, it has no MeshFilter and LODGroup component.";
        private const string CANT_SELECT_PREFAB_INDEX_GREATER = "MBS. Asset name: {0}. Can't select prefab, given @prefabIndex: {1} is more than Prefabs count.";
        private const string CANT_SELECT_PREFAB_INDEX_LESSZERO = "MBS. Asset name: {0}. Can't select prefab, given @prefabIndex: {1} is less than 0.";
        private const string CANT_SELECT_PREFAB_NO_MESH = "MBS. Asset name: {0}. Can't select prefab at @prefabIndex: {1}, the MeshFilter component has no @sharedMesh.";
        private const string CANT_SELECT_PREFAB_NO_CHILDREN = "MBS.  Asset name: {0}. Can't select prefab at @prefabIndex: {1}, it has LODGroup but no children objects.";
        private const string CANT_SELECT_PREFAB_MISSING = "MBS. Asset name: {0}. Can't select prefab at @prefabIndex {1}, reference is missing (null).";
        private const string CANT_SELECT_PREFAB_LOD_CHILDREN_NO_MESH = "MBS.  Asset name: {0}. Can't select prefab at @prefabIndex: {1}, it has LODGroup but no children has MeshFilter or attached mesh.";
        [SerializeField] public string name;
        [SerializeField] public string guid;
        [SerializeField] public GameObject[] prefabs;

        public virtual GameObject[] Prefabs
        {
            get
            {
                if (prefabs == null)
                    prefabs = new GameObject[0];
                return prefabs;
            }
        }

        public MBSAsset()
        {
            guid = Guid.NewGuid().ToString("N");
        }

        public virtual void HealthCheckAndFix()
        {
            if (string.IsNullOrEmpty(guid))
                guid = Guid.NewGuid().ToString("N");
        }

        public bool TryGetPrefab(int prefabIndex, out GameObject prefab, out PrefabType prefabType, bool debugErrors = true)
        {
            prefab = null;
            prefabType = PrefabType.None;

            if (prefabIndex < 0)
            {
                if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_INDEX_LESSZERO, this.name, prefabIndex));
                return false;
            }

            if (prefabIndex >= Prefabs.Length)
            {
                if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_INDEX_GREATER, this.name, prefabIndex));
                return false;
            }

            prefab = Prefabs.ElementAt(prefabIndex);

            if (prefab != null)
            {
                if (prefab.TryGetComponent<MeshFilter>(out MeshFilter mf))
                {
                    if (mf.sharedMesh != null)
                    {
                        prefabType = PrefabType.Basic;
                        return true;
                    }
                    else
                    {
                        if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_NO_MESH, this.name, prefabIndex));
                        return false;
                    }
                }
                else if (prefab.TryGetComponent<LODGroup>(out LODGroup lg))
                {
                    if (prefab.transform.childCount > 0)
                    {
                        foreach (Transform child in prefab.transform)
                        {
                            if (child.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                            {
                                if (meshFilter.sharedMesh != null)
                                {
                                    prefabType = PrefabType.LodGroup;
                                    return true;
                                }
                            }
                        }
                        if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_LOD_CHILDREN_NO_MESH, this.name, prefabIndex));
                        return false;
                    }
                    else
                    {
                        if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_NO_CHILDREN, this.name, prefabIndex));
                        return false;
                    }
                }
                else
                    if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_EMPTY, this.name, prefabIndex));
            }
            else
                if (debugErrors) Debug.LogError(string.Format(CANT_SELECT_PREFAB_MISSING, this.name, prefabIndex));
            return false;
        }
    }


    [Serializable]
    public class MBSWallAsset : MBSAsset
    {
        public override GameObject[] Prefabs
        {
            get
            {
                if (prefabs != null)
                {
                    List<GameObject> temp = prefabs.ToList();
                    temp.RemoveAll(i => i == null);
                    prefabs = temp.ToArray();
                }
                else prefabs = new GameObject[0];
                return prefabs;
            }
        }

        public MBSWallAsset() : base() { }

        public MBSWallAsset(SortedDictionary<int, GameObject> fillDict) : base()
        {
            prefabs = new GameObject[fillDict.Count];

            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i] = fillDict.ElementAtOrDefault(i).Value;
            }
        }

        public void FillWithFindedPrefabs(SortedDictionary<int, GameObject> fillDict)
        {
            if (prefabs == null || prefabs.Length < fillDict.Count)
                prefabs = new GameObject[fillDict.Count];

            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i] = fillDict.ElementAtOrDefault(i).Value;
            }
        }

        public bool IsEmpty()
        {
            this.HealthCheckAndFix();

            if (prefabs.Length == 0)
                return true;

            return false;
        }

        public override void HealthCheckAndFix()
        {
            base.HealthCheckAndFix();

            if (prefabs != null)
            {
                List<GameObject> temp = prefabs.ToList();
                temp.RemoveAll(i => i == null);
                prefabs = temp.ToArray();
            }
            else prefabs = new GameObject[0];
        }
    }


    [Serializable]
    public class MBSFloorAsset : MBSAsset
    {
        public override GameObject[] Prefabs
        {
            get => prefabs;
        }

        public MBSFloorAsset() : base() { }

        public MBSFloorAsset(SortedDictionary<int, GameObject> fillDict) : base()
        {
            prefabs = new GameObject[2];

            if (fillDict.Count > 0)
                prefabs[0] = fillDict.ElementAtOrDefault(0).Value;
            if (fillDict.Count > 1)
                prefabs[1] = fillDict.ElementAtOrDefault(1).Value;
        }

        public void FillWithFindedPrefabs(SortedDictionary<int, GameObject> fillDict)
        {
            if (prefabs == null || prefabs.Length != 2)
                prefabs = new GameObject[2];

            if (fillDict.Count > 0)
                prefabs[0] = fillDict.ElementAtOrDefault(0).Value;
            if (fillDict.Count > 1)
                prefabs[1] = fillDict.ElementAtOrDefault(1).Value;
        }

        public bool IsEmpty()
        {
            if (prefabs == null || prefabs.ElementAtOrDefault(0) == null || prefabs.ElementAtOrDefault(1) == null)
                return true;

            return false;
        }
    }
}
#endif
