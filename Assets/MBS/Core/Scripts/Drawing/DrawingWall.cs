#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace MBS
{
    [Serializable, ExecuteInEditMode]
    public class DrawingWall : EditorBehaviour
    {
        private const string ASSET_GUID_EMPTY = "MBS. DrawingWall. Fatal Error. Original Asset Guid is empty, can not load the original asset. If you see this, probably you resseted drawing wall component.";
        private const string CANT_INIT_ENDPOINTS_ZERO_SIZE = "MBS. Can't initialize end-points of a DrawingWall, the @assetSize is zero vector.";

        [SerializeField] public bool instantiated;

        [SerializeField] private MBSBuilder _builder;
        [SerializeField] private bool _is45Degree;
        [SerializeField] private Vector3 _assetSize;
        [SerializeField] private string _originalAssetGUID;
        [SerializeField] private Mesh _originalMesh;
        [NonSerialized] private MBSWallAsset _originalAsset;

        [SerializeField] private PrefabType _prefabType;
        [SerializeField] private int _currentPrefabIndex;

        [SerializeField] public Vector3 _frontEndPoint;
        [SerializeField] public Vector3 _rearEndPoint;
        [SerializeField] public List<DrawingWall> _frontConnections;
        [SerializeField] public List<DrawingWall> _rearConnections;
        [SerializeField] public MeshSideModification _frontSideModification;
        [SerializeField] public MeshSideModification _rearSideModification;
        [NonSerialized] private List<Area> _attachedAreas;

        [SerializeField] private GameObject _connectedTo;

        public MBSBuilder Builder
        {
            get => _builder;
            set => _builder = value;
        }

        public MBSWallAsset OriginalAsset
        {
            get
            {
                if (_originalAsset == null)
                {
                    if (!string.IsNullOrEmpty(OriginalAssetGUID))
                        return (MBSWallAsset)MBSAssetsManager.Singleton.GetAssetByGUID(OriginalAssetGUID);
                    else
                    {
                        Debug.LogError(ASSET_GUID_EMPTY);
                        return null;
                    }
                }
                return _originalAsset;
            }
        }
        public string OriginalAssetGUID { get => _originalAssetGUID; }
        public int CurrentPrefabIndex { get => _currentPrefabIndex; }
        public Vector3 AssetSize { get => _assetSize; }



        public Vector3 FrontEndPoint => _frontEndPoint;
        public Vector3 RearEndPoint => _rearEndPoint;

        public List<DrawingWall> FrontConnections
        {
            get
            {
                if (_frontConnections == null)
                    _frontConnections = new List<DrawingWall>();
                else _frontConnections.RemoveAll(i => i == null);
                return _frontConnections;
            }
            set
            {
                _frontConnections = value;
            }
        }
        public List<DrawingWall> RearConnections
        {
            get
            {
                if (_rearConnections == null)
                    _rearConnections = new List<DrawingWall>();
                else _rearConnections.RemoveAll(i => i == null);
                return _rearConnections;
            }
            set
            {
                _rearConnections = value;
            }
        }

        public MeshSideModification FrontSideModification
        {
            get
            {
                if (_frontSideModification == null)
                    _frontSideModification = new MeshSideModification(0, 0, 0);
                return _frontSideModification;
            }
            set => _frontSideModification = value;
        }
        public MeshSideModification RearSideModification
        {
            get
            {
                if (_rearSideModification == null)
                    _rearSideModification = new MeshSideModification(0, 0, 0);
                return _rearSideModification;
            }
            set => _rearSideModification = value;
        }

        public List<Area> AttachedAreas
        {
            get
            {
                if (_attachedAreas == null)
                    _attachedAreas = new List<Area>();
                return _attachedAreas;
            }
            set => _attachedAreas = value;
        }


        public void OnEnable()
        {
            name = "DrawingWall";

            if (!instantiated) return;
            if (_builder != null) InitializeEndPoints();
        }
        public override void DoOnDestroy()
        {
            if (_builder == null || _builder._sceneData == null || _builder._sceneData.isDrawingMode || MBSConfig.Singleton.pluginDisabled)
                return;

            _builder._drawingManager.RemoveDrawingItem(this, true);
            RemoveAllAreas();
            ClearConnections();
        }
        public override void DoOnSoftDelete()
        {
            if (_builder == null || _builder._sceneData == null || _builder._sceneData.isDrawingMode || MBSConfig.Singleton.pluginDisabled)
                return;

            _builder._drawingManager.RemoveDrawingItem(this, true);

            if (AttachedAreas.Count > 0)
            {
                for (int i = 0; i < AttachedAreas.Count; i++)
                {
                    AttachedAreas[i].OnDestroyItem(this);
                }
            }
        }

        public void InitializeAfterDrawing(MBSBuilder _builder, MBSAsset asset, Vector3 assetSize, PrefabType prefabType, bool is45Degree)
        {
            instantiated = true;
            this._builder = _builder;
            this._originalAsset = (MBSWallAsset)asset;
            this._originalAssetGUID = asset.guid;
            this._assetSize = assetSize;
            this._prefabType = prefabType;
            this._is45Degree = is45Degree;

            SetupOriginalMesh();
            InitializeEndPoints();
            InitializeDrawingChilds();

            if (this._is45Degree)
            {
                transform.localScale = Vector3.one;
                this._assetSize.x = (this._assetSize.x / Mathf.Sin(45 * Mathf.Deg2Rad));
                UpdateMesh();
            }
        }
        public void InitializeAfterChanging(MBSBuilder _builder, MBSAsset asset, int prefabIndex, Vector3 assetSize, PrefabType prefabType, bool is45Degree)
        {
            instantiated = true;
            this._builder = _builder;
            this._originalAsset = (MBSWallAsset)asset;
            this._originalAssetGUID = asset.guid;
            this._assetSize = assetSize;
            this._currentPrefabIndex = prefabIndex;
            this._prefabType = prefabType;
            this._is45Degree = is45Degree;

            InitializeEndPoints();
            SetupOriginalMesh();
            InitializeDrawingChilds();
        }
        public void InitializeDrawingChilds()
        {
            transform.DoRecursive((Transform t) =>
            {
                if (t.TryGetComponent(out MeshFilter meshFilter))
                {
                    bool isDwc = t.TryGetComponent(out DrawingWallChild dwc);
                    if (!isDwc)
                    {
                        dwc = t.gameObject.AddComponent<DrawingWallChild>();
                        dwc.RootDW = this;
                    }
                    dwc._originalMesh = meshFilter.sharedMesh;
                }
            });
        }
        private void InitializeEndPoints()
        {
            if (_assetSize == Vector3.zero)
            {
                Debug.LogError(CANT_INIT_ENDPOINTS_ZERO_SIZE);
                return;
            }

            _frontEndPoint = this.transform.position + ((_assetSize.x / 2 * this.transform.localScale.x) * this.transform.right);
            _frontEndPoint = _builder.LPos(FrontEndPoint);

            _rearEndPoint = this.transform.position - ((_assetSize.x / 2 * this.transform.localScale.x) * this.transform.right);
            _rearEndPoint = _builder.LPos(RearEndPoint);
        }


        public void RecalculateBothSides(bool doUpdateMesh = true)
        {
            if (_builder == null) return;

            CalculateSide(this, FrontConnections, _builder.WPos(FrontEndPoint), doUpdateMesh);
            CalculateSide(this, RearConnections, _builder.WPos(RearEndPoint), doUpdateMesh);
        }
        public void CalculateSide(DrawingWall actualWall, List<DrawingWall> connectedWalls, Vector3 connectionPoint_W, bool doUpdateMesh)
        {
            if (_builder == null) return;

            List<DrawingWall> allItems = new List<DrawingWall>(connectedWalls);
            allItems.Add(actualWall);

            float startSide = 0;
            bool areAllInOneSide = false;
            int firstItemIndex = -1;
            DrawingWall firstItem = null;

            for (int i = 0; i < allItems.Count; i++)
            {
                DrawingWall dw_i = allItems[i];
                int nextIndex = (i + 1) % allItems.Count;

                if (nextIndex == i || allItems.Count < nextIndex)
                    continue;

                areAllInOneSide = true;


                float crossProduct = GetSideRegardToLine(connectionPoint_W, dw_i.transform.position, allItems[nextIndex].transform.position);
                startSide = crossProduct;

                for (int j = 0; j < allItems.Count; j++)
                {
                    if (i == j || i == nextIndex) continue;

                    crossProduct = GetSideRegardToLine(connectionPoint_W, dw_i.transform.position, allItems[j].transform.position);
                    areAllInOneSide &= (crossProduct >= 0 && startSide >= 0) || (crossProduct <= 0 && startSide <= 0);
                }

                if (areAllInOneSide)
                {
                    firstItemIndex = i;
                    firstItem = allItems[firstItemIndex];
                    break;
                }
            }

            if (areAllInOneSide)
            {
                float maxAngle = float.MinValue;
                DrawingWall secondItem = null;
                int secondItemIndex = -1;

                for (int i = 0; i < allItems.Count; i++)
                {
                    if (firstItemIndex == i || allItems[i] == firstItem) continue;

                    Vector3 sideA = allItems[firstItemIndex].transform.position - connectionPoint_W;
                    Vector3 sideB = allItems[i].transform.position - connectionPoint_W;
                    float currentAngle = Vector3.Angle(sideA, sideB).RoundDecimals(3);

                    if (currentAngle > maxAngle)
                    {
                        maxAngle = currentAngle;
                        secondItem = allItems[i];
                        secondItemIndex = i;
                    }
                }

                if (secondItem != null)
                {
                    if (maxAngle != 180 && maxAngle != 0)
                    {
                        for (int i = 0; i < allItems.Count; i++)
                        {
                            if (i != secondItemIndex && i != firstItemIndex)
                            {
                                allItems[i].ResetSideModification(connectionPoint_W);
                                if (doUpdateMesh) allItems[i].UpdateMesh();
                            }
                        }

                        firstItem.UpdateSideModification(secondItem, connectionPoint_W, maxAngle);
                        secondItem.UpdateSideModification(firstItem, connectionPoint_W, maxAngle);

                        if (doUpdateMesh)
                        {
                            firstItem.UpdateMesh();
                            secondItem.UpdateMesh();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < allItems.Count; i++)
                        {
                            allItems[i].ResetSideModification(connectionPoint_W);
                            if (doUpdateMesh) allItems[i].UpdateMesh();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < allItems.Count; i++)
                {
                    allItems[i].ResetSideModification(connectionPoint_W);
                    if (doUpdateMesh) allItems[i].UpdateMesh();
                }
            }
        }

        private void UpdateSideModification(DrawingWall connectedWall, Vector3 connectionPoint, float angle)
        {
            if (_builder == null) return;

            InitializeEndPoints();

            MeshSideModification modification = null;

            if (_builder.LPos(connectionPoint) == FrontEndPoint)
                modification = FrontSideModification;
            else if (_builder.LPos(connectionPoint) == RearEndPoint)
                modification = RearSideModification;
            else
            {
                Debug.LogError(string.Format("MBS. Connection point does not equal nor front or rear end point. " +
                                            "ConnectionPoint: {0}. FrontPoint: {1}. RearPoint: {2}.", _builder.LPos(connectionPoint), FrontEndPoint, RearEndPoint));
                return;
            }


            float dist = Vector3.Distance(this.transform.position, connectedWall.transform.position);
            float dist1 = Vector3.Distance(this.transform.position + (this.transform.forward / 2), connectedWall.transform.position);

            if (dist1 < dist)
            {
                modification.positiveSide = -1;
                modification.negativeSide = +1;
                modification.angle = angle;
            }
            else if (dist1 > dist)
            {
                modification.positiveSide = +1;
                modification.negativeSide = -1;
                modification.angle = angle;
            }
        }
        private void ResetSideModification(Vector3 connectionPoint)
        {
            if (_builder == null) return;
            if (_builder.LPos(connectionPoint) == FrontEndPoint)
            {
                _frontSideModification = new MeshSideModification(0, 0, 0);
            }
            else if (_builder.LPos(connectionPoint) == RearEndPoint)
            {
                _rearSideModification = new MeshSideModification(0, 0, 0);
            }
            else
            {
                Debug.LogError(string.Format("MBS. Connection point does not equal nor front or rear end point. " +
                                            "ConnectionPoint: {0}. FrontPoint: {1}. RearPoint: {2}.", _builder.LPos(connectionPoint), FrontEndPoint, RearEndPoint));
                return;
            }
        }
        private void ResetSideModifications()
        {
            _frontSideModification = new MeshSideModification(0, 0, 0);
            _rearSideModification = new MeshSideModification(0, 0, 0);
        }

        public float GetSideRegardToLine(Vector3 startLine, Vector3 endLine, Vector3 point)
        {
            float retval = Vector3.Cross(startLine - point, endLine - point).y.RoundDecimals();
            return retval;
        }


        public void UpdateMesh()
        {
            if (_builder == null) return;
            if (_prefabType == PrefabType.Basic)
            {
                Mesh mesh = WallMeshModifier.ModifyMesh(_originalMesh,
                                                        FrontSideModification,
                                                        RearSideModification,
                                                        this.transform.InverseTransformPoint(_builder.WPos(FrontEndPoint)),
                                                        this.transform.InverseTransformPoint(_builder.WPos(RearEndPoint)),
                                                        transform,
                                                        transform,
                                                        _is45Degree);
                SetupModifiedMesh(mesh);
            }

            this.transform.DoRecursive((Transform t) =>
            {
                if (t.TryGetComponent<DrawingWallChild>(out DrawingWallChild dwc))
                {
                    if (!dwc._doModify)
                        dwc.SetupMesh();
                    else
                    {
                        Mesh modifiedMesh = WallMeshModifier.ModifyMesh(dwc._originalMesh,
                                                                        FrontSideModification,
                                                                        RearSideModification,
                                                                        this.transform.InverseTransformPoint(_builder.WPos(FrontEndPoint)),
                                                                        this.transform.InverseTransformPoint(_builder.WPos(RearEndPoint)),
                                                                        transform,
                                                                        dwc.transform,
                                                                        _is45Degree);
                        dwc.SetupMesh(modifiedMesh);
                    }


                }
            });
        }
        private void SetupModifiedMesh(Mesh mesh)
        {
            if (TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter.mesh = mesh;
            }

            MeshCollider meshCollider = GetComponent<MeshCollider>();

            if (meshCollider == null)
                meshCollider = this.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
        private void SetupOriginalMesh()
        {
            if (_prefabType == PrefabType.Basic)
                if (TryGetComponent(out MeshFilter meshFilter))
                {
                    _originalMesh = meshFilter.sharedMesh;
                }
        }


        public void AttachArea(Area area)
        {
            if (_builder == null) return;
            if (area == null) return;
            bool IsThereEqualAreaItems = false;

            foreach (Area a in AttachedAreas)
            {
                IsThereEqualAreaItems |= AreaManager.IsTwoAreasEqual(a, area);
            }
            if (!IsThereEqualAreaItems)
            {
                AttachedAreas.Add(area);
            }
        }
        public void DetachArea(Area area)
        {
            if (_builder == null) return;
            if (area == null) return;
            if (AttachedAreas.Count == 0) return;
            AttachedAreas = AttachedAreas.Where(i => i != area).ToList();
        }
        public void RemoveAllAreas()
        {
            if (_builder == null) return;
            if (AttachedAreas.Count > 0)
            {
                for (int i = 0; i < AttachedAreas.Count; i++)
                {
                    AttachedAreas[i].OnDestroyItem(this);
                }
            }
        }


        public void AddFrontConnection(DrawingWall item)
        {
            InitializeEndPoints();
            if (item != null && item != this && !FrontConnections.Contains(item))
            {
                FrontConnections.Add(item);
            }
        }
        public void AddRearConnection(DrawingWall item)
        {
            InitializeEndPoints();
            if (item != null && item != this && !RearConnections.Contains(item))
            {
                RearConnections.Add(item);
            }
        }
        private void RemoveFrontConnectionIfExist(DrawingWall item)
        {
            if (item != null && FrontConnections.Contains(item))
            {
                FrontConnections.Remove(item);
            }
        }
        private void RemoveRearConnectionIfExist(DrawingWall item)
        {
            if (item != null && RearConnections.Contains(item))
            {
                RearConnections.Remove(item);
            }
        }
        public void RemoveConnection(DrawingWall item, bool doRecalculateSides)
        {
            RemoveFrontConnectionIfExist(item);
            RemoveRearConnectionIfExist(item);
            if (doRecalculateSides) RecalculateBothSides();
        }
        public void ClearConnections()
        {
            _frontConnections = new List<DrawingWall>();
            _rearConnections = new List<DrawingWall>();
        }


        public DrawingWall ChangeModel(int prefabIndex, bool multipleSelection)
        {
            if (_builder == null) return this;
            if (OriginalAsset.IsEmpty())
            {
                Debug.LogError(CANT_INIT_ENDPOINTS_ZERO_SIZE);
                return this;
            }

            if (OriginalAsset.TryGetPrefab(prefabIndex, out GameObject chosenPrefab, out PrefabType type))
            {
                GameObject changedPrefab = Instantiate(chosenPrefab);
                changedPrefab.transform.SetParent(this.transform.parent);
                changedPrefab.transform.localPosition = this.transform.localPosition;
                changedPrefab.transform.localRotation = this.transform.localRotation;
                changedPrefab.transform.localScale = this.transform.localScale;
                changedPrefab.RecordCreatedUndo("MBS. Changing wall prefab");

                DrawingWall changed_dw = changedPrefab.AddComponent<DrawingWall>();
                changed_dw._builder = this._builder;
                changed_dw.tag = this.tag;
                changed_dw.name = this.name;

                changed_dw.InitializeAfterChanging(_builder, OriginalAsset, prefabIndex, _assetSize, _prefabType, _is45Degree);

                if (multipleSelection)
                {
                    _builder._drawingManager.AddDrawingWall(changed_dw, false);
                }
                else
                {
                    _builder._drawingManager.RemoveDrawingItem(this, false);
                    this.RemoveAllAreas();
                    this.ClearConnections();
                    _builder._drawingManager.AddDrawingWall(changed_dw, true);

                    changedPrefab.AddToSelection();

                    this.gameObject.DestroyImmediateUndo();
                }
                return changed_dw;
            }
            return this;
        }
        public void FaceWallInsideArea(Area area)
        {
            if (_builder == null) return;
            float checkDistance = 0.2f;
            Vector3 inFacePos = transform.position + (transform.forward * checkDistance);
            Vector3 ofFacePos = transform.position - (transform.forward * checkDistance);

            bool area1 = area.IsPointInsideArea(_builder.LPos(inFacePos));
            bool area2 = area.IsPointInsideArea(_builder.LPos(ofFacePos));


            _builder._drawingManager.RemoveDrawingItem(this, false);
            this.ResetSideModifications();
            this.ClearConnections();

            if (area2)
            {
                this.transform.Rotate(0, 180, 0);
                InitializeEndPoints();
            }
        }
    }


    [Serializable]
    public class MeshSideModification
    {
        public float angle;
        public float positiveSide;
        public float negativeSide;

        public MeshSideModification(float angle, float positiveSide, float negativeSide)
        {
            this.angle = angle;
            this.positiveSide = positiveSide;
            this.negativeSide = negativeSide;
        }
    }
}


#endif
