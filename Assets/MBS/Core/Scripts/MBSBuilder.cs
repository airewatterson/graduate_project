#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace MBS
{
    [ExecuteInEditMode]
    public class MBSBuilder : EditorBehaviour, ISerializationCallbackReceiver
    {
        [NonSerialized] public bool AFTER_DESERIALIZATION = false;
        [NonSerialized] public bool _awaken = false;

        [NonSerialized] private Mesh _gizmoWall;
        [NonSerialized] private Mesh _gizmoFloor;
        [NonSerialized] private Vector3 _gizmoPos;
        [NonSerialized] private Vector3 _gizmoRot;
        [NonSerialized] private Color _gizmoColor;

        [NonSerialized] public SceneData _sceneData;
        [NonSerialized] public AssetsData _assetsData;
        [NonSerialized] public DrawingManager _drawingManager;
        [NonSerialized] public AreaManager _areaManager;
        [NonSerialized] public MBSGridManager _gridManager;
        [SerializeField] public AssetsDataSerializable _assetsDataSerializable;

        public Vector3 GizmoPos
        {
            get => _gizmoPos;
            set => _gizmoPos = value;
        }
        public Vector3 GizmoRot
        {
            get => _gizmoRot;
            set => _gizmoRot = value;
        }

        public Vector3 LPos(Vector3 worldPos) => this.transform.InverseTransformPoint(worldPos).RoundDecimals(5);
        public Vector3 WPos(Vector3 localPos) => this.transform.TransformPoint(localPos);

        public void SoftInitialization()
        {
            MBSAssetsManager.Singleton.RefreshAssetPacks();

            if (_assetsData == null)
                _assetsData = new AssetsData(this);
            else _assetsData.Initialize();

            if (_sceneData == null)
                _sceneData = new SceneData();

            if (_areaManager == null)
                _areaManager = new AreaManager(this);

            if (_gridManager == null)
                _gridManager = new MBSGridManager(this);
            _gridManager.Initialize();

            if (_drawingManager == null)
                _drawingManager = new DrawingManager(this);
            if (AFTER_DESERIALIZATION) _drawingManager.Initialize();

            AFTER_DESERIALIZATION = false;
            _awaken = true;

            ChangeGizmo(_assetsData.Current_Tool);
            SetGizmoColorUnsnaped();
            GridMan_SetParams(_assetsData);

            this.transform.GetComponent<MeshFilter>().sharedMesh.bounds = new Bounds(Vector3.zero, (Vector3.forward + Vector3.right) * 30);
        }
        public void HardInitialization()
        {
            MBSAssetsManager.Singleton.RefreshAssetPacks();

            _gridManager = new MBSGridManager(this);
            _gridManager.Initialize();

            _sceneData = new SceneData();
            _assetsData = new AssetsData(this);
            _areaManager = new AreaManager(this);
            _drawingManager = new DrawingManager(this);
            _drawingManager.Initialize();

            _gizmoPos = new Vector3();
            _gizmoRot = new Vector3();

            ChangeGizmo(_assetsData.Current_Tool);
            SetGizmoColorUnsnaped();
            GridMan_SetParams(_assetsData);
        }

        public override void DoOnDestroy()
        {
            DestroySerializableObjects();
        }
        public override void DoOnSoftDelete()
        {
            DestroySerializableObjects();
        }
        public void OnDrawGizmosSelected()
        {
            Vector3 originPoint = new Vector3(this.transform.position.x, _gridManager.Height, this.transform.position.z);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(originPoint, 0.1f);
            Handles.color = Color.green;
            Handles.DrawLine(originPoint, originPoint + this.transform.up);
            Handles.color = Color.red;
            Handles.DrawLine(originPoint, originPoint + this.transform.right);
            Handles.color = Color.cyan;
            Handles.DrawLine(originPoint, originPoint + this.transform.forward);

            if (_gridManager.Height > 0)
            {
                Handles.color = new Color(0.25f, 0.25f, 0.25f);
                Handles.DrawDottedLine(Vector3.up * _gridManager.Height, Vector3.zero, 6);

                Vector3 secondOrigin = this.transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(secondOrigin, 0.1f);
                Handles.color = Color.green;
                Handles.DrawLine(secondOrigin, secondOrigin + this.transform.up);
                Handles.color = Color.red;
                Handles.DrawLine(secondOrigin, secondOrigin + this.transform.right);
                Handles.color = Color.cyan;
                Handles.DrawLine(secondOrigin, secondOrigin + this.transform.forward);

            }

            switch (_sceneData.sceneMode)
            {
                case SceneViewMode.Idle:
                case SceneViewMode.Drawing:
                    switch (_assetsData.Current_Tool)
                    {
                        case BuilderTools.Walls:
                            Gizmos.color = _gizmoColor;
                            Gizmos.DrawMesh(_gizmoWall, _gizmoPos, Quaternion.Euler(_gizmoRot));
                            if (_assetsData.CalcLevelHeight > 0)
                            {
                                Handles.color = _gizmoColor;
                                Handles.DrawLine(_gizmoPos, _gizmoPos + (Vector3.down * _assetsData.CalcLevelHeight));
                            }
                            break;
                        case BuilderTools.Floors:
                            Gizmos.color = _gizmoColor;
                            Vector3 floorPos = _gizmoPos;
                            floorPos.y += 0.005f;
                            Gizmos.DrawMesh(_gizmoFloor, floorPos, Quaternion.Euler(_gizmoRot + this.transform.localRotation.eulerAngles));
                            break;
                    }
                    break;
                case SceneViewMode.PositionPicker:
                    float hSize = HandleUtility.GetHandleSize(_gizmoPos) * 2;
                    Handles.color = _gizmoColor;
                    Handles.DrawLine(_sceneData.pickerPos - this.transform.up / 3, _sceneData.pickerPos + this.transform.up / 3);
                    Handles.DrawLine(_sceneData.pickerPos - this.transform.right / 3, _sceneData.pickerPos + this.transform.right / 3);
                    Handles.DrawLine(_sceneData.pickerPos - this.transform.forward / 3, _sceneData.pickerPos + this.transform.forward / 3);
                    Gizmos.color = _gizmoColor;
                    Gizmos.DrawSphere(_sceneData.pickerPos, 0.08f);
                    break;
                case SceneViewMode.ObjectPicker:
                    if (_sceneData.ObjPicker_Object == null) return;
                    Gizmos.color = Color.white;
                    Vector3 ext = _sceneData.ObjPicker_ObjExtents * 0.75f;
                    Vector3 leftBot = _sceneData.ObjPicker_ObjCenterLocal + new Vector3(-ext.x, -ext.y, 0);
                    Vector3 leftTop = _sceneData.ObjPicker_ObjCenterLocal + new Vector3(-ext.x, ext.y, 0);
                    Vector3 rightTop = _sceneData.ObjPicker_ObjCenterLocal + new Vector3(ext.x, ext.y, 0);
                    Vector3 rightBot = _sceneData.ObjPicker_ObjCenterLocal + new Vector3(ext.x, -ext.y, 0);
                    leftBot = _sceneData.ObjPicker_Object.transform.TransformPoint(leftBot);
                    leftTop = _sceneData.ObjPicker_Object.transform.TransformPoint(leftTop);
                    rightTop = _sceneData.ObjPicker_Object.transform.TransformPoint(rightTop);
                    rightBot = _sceneData.ObjPicker_Object.transform.TransformPoint(rightBot);

                    Gizmos.DrawLine(leftBot, leftTop);
                    Gizmos.DrawLine(leftTop, rightTop);
                    Gizmos.DrawLine(rightTop, rightBot);
                    Gizmos.DrawLine(rightBot, leftBot);
                    Gizmos.DrawSphere(_sceneData.ObjPicker_Object.transform.TransformPoint(_sceneData.ObjPicker_ObjCenterLocal), 0.08f);
                    break;
            }


        }
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_assetsData != null)
                _assetsDataSerializable = _assetsData.GetSerializable(_assetsDataSerializable);
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            AFTER_DESERIALIZATION = true;

            _awaken = false;

            if (_assetsDataSerializable != null)
                _assetsData = new AssetsData(this, _assetsDataSerializable);

            _drawingManager = new DrawingManager(this);
            _areaManager = new AreaManager(this);
            _sceneData = new SceneData();
            _gridManager = new MBSGridManager(this);
        }

        private void DestroySerializableObjects()
        {
            if (_assetsDataSerializable != null) Undo.DestroyObjectImmediate(_assetsDataSerializable);
        }

        public void SetGizmoColorSnapped()
        {
            _gizmoColor = new Color(1, .1f, 0, 1);
        }
        public void SetGizmoColorUnsnaped()
        {
            _gizmoColor = Color.green;
        }

        public void SetFloorGizmoMesh(FloorTileType floorType = FloorTileType.Square)
        {
            _gizmoFloor = _assetsData.Current_Asset_MeshFilter(floorType)?.sharedMesh;
        }
        public void SetWallGizmoHeight()
        {
            if (_gizmoWall == null)
                if (MBSConfig.Singleton != null && MBSConfig.Singleton.wallGizmoMesh != null)
                    _gizmoWall = MBSConfig.Singleton.wallGizmoMesh;


            if (_gizmoWall != null && _gizmoWall.colors != null && _gizmoWall.colors.Length > 0)
            {
                Vector3[] vertecies = _gizmoWall.vertices;
                Color[] colors = _gizmoWall.colors;
                Color yellow = new Color(1, 1, 0, 1);
                float height = _assetsData.Current_Asset_Size.y + .2f;
                height = Mathf.Max(height, 1);
                for (int i = 0; i < vertecies.Length; i++)
                {
                    if (colors[i] == yellow)
                        vertecies[i] = new Vector3(vertecies[i].x, height, vertecies[i].z);

                }
                _gizmoWall.vertices = vertecies;
            }
        }
        public void ChangeGizmo(BuilderTools tool)
        {
            switch (tool)
            {
                case BuilderTools.Walls:
                    SetWallGizmoHeight();
                    break;

                case BuilderTools.Floors:
                    SetFloorGizmoMesh();
                    break;
            }
        }


        public void GridMan_SetParams(AssetsData assetsData)
        {
            if (assetsData == null) return;
            if (_gridManager == null) SoftInitialization();

            _gridManager.GridSize = assetsData.GridSize;
            _gridManager.Position = assetsData.GridPosition;
            _gridManager.Height = assetsData.CalcLevelHeight;
        }
        public void GridMan_SetParams(float gridSize = float.MinValue, float levelHeight = float.MinValue, int levelNumber = int.MinValue, Vector3? gridPos = default)
        {
            if (_gridManager == null) SoftInitialization();

            if (gridSize != float.MinValue)
            {
                _assetsData.GridSize = gridSize;
                _gridManager.GridSize = _assetsData.GridSize;
            }
            if (levelHeight != float.MinValue)
            {
                _assetsData.LevelHeight = levelHeight;
                _gridManager.Height = _assetsData.CalcLevelHeight;
            }
            if (levelNumber != int.MinValue)
            {
                _assetsData.LevelNumber = levelNumber;
                _gridManager.Height = _assetsData.CalcLevelHeight;
            }

            if (gridPos != null)
            {
                _assetsData.GridPosition = (Vector3)gridPos;
                _gridManager.Position = (Vector3)gridPos;
                _gridManager.Height = _assetsData.CalcLevelHeight;
            }
        }
    }
}
#endif