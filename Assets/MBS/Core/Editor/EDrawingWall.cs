using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace MBS
{
    [CustomEditor(typeof(DrawingWall)), CanEditMultipleObjects]
    internal class EDrawingItem : Editor
    {
        private const string HELPBOX_COMPONENTS_DISABLED = "All MBS Components disabled. To enable do Tools -> MBS -> Enable All Components";
        private const string HELPBOX_BUILDER_MISSING = "MBSBuilder this wall(s) attached to is missing, can't work without it.";
        private const string HELPBOX_ASSET_NULL = "Can't reach original MBSAsset of selected component(s), can't work without it.";
        private const string HELPBOX_DIFFERENT_ASSETS = "Selected walls are using different assets, please select walls with the same assets.";
        private const string HELPBOX_DIFFERENT_BUILDERS = "Selected walls are attached to different MBSBuilder's, to edit walls please select walls attached to the same MBSBuilder.";
        private const string UNDO_MULTIPLE_PREFAB_CHANGING = "MBS. Multiple Wall Prefab Changing";



        private DrawingWall _single;
        private List<DrawingWall> _multiple;
        private List<DrawingWallChild> _singleChildren;
        private List<List<DrawingWallChild>> _multipleChildren;
        private List<bool> _multiple_foldouts;
        private MBSWallAsset _originalAsset;

        private Vector2 _scrollPos;

        private float _windowWidth;
        private bool _foldoutChildren;
        private bool _foldoutAreas;


        private bool _builderNull;
        private bool _sameBuilder;
        private bool _assetNull;
        private bool _sameAsset;
        private bool _samePrefabIndex;
        private int _prefabIndex;

        private bool _isMultiple;
        private bool _multipleChildrenMoreThanOne;


        private GUIStyle boldFoldout;


        private void OnEnable()
        {
            if (targets.Length == 1) _isMultiple = false;
            else _isMultiple = true;

            if (!_isMultiple)
            {
                _single = (DrawingWall)target;
                _builderNull = _single.Builder == null;

                if (!_builderNull)
                {
                    _originalAsset = _single.OriginalAsset;
                    _assetNull = _originalAsset == null;

                    if (!_assetNull)
                    {
                        _singleChildren = new List<DrawingWallChild>();
                        _single.transform.DoRecursive((Transform t) =>
                        {
                            if (t.TryGetComponent<DrawingWallChild>(out DrawingWallChild child))
                            {
                                _singleChildren.Add(child);
                            }
                        });

                        if (!_builderNull && !_single.Builder._awaken)
                            _single.Builder.SoftInitialization();
                    }
                }
            }
            else
            if (_isMultiple)
            {
                _multiple = targets.Cast<DrawingWall>().ToList();
                _builderNull = _multiple.Any(i => i.Builder == null);

                if (!_builderNull)
                {
                    MBSBuilder initialBuilder = _multiple[0].Builder;
                    _sameBuilder = _multiple.All(i => i.Builder == initialBuilder);

                    if (_sameBuilder)
                    {
                        _assetNull = _multiple.Any(i => i.OriginalAsset == null);

                        if (!_assetNull)
                        {
                            _originalAsset = _multiple[0].OriginalAsset;
                            _sameAsset = _multiple.All(i => i.OriginalAsset == _originalAsset);

                            _prefabIndex = _multiple[0].CurrentPrefabIndex;
                            _samePrefabIndex = _multiple.All(i => i.CurrentPrefabIndex == _prefabIndex);

                            _multipleChildren = new List<List<DrawingWallChild>>();
                            _multiple_foldouts = new List<bool>();

                            for (int i = 0; i < _multiple.Count; i++)
                            {
                                List<DrawingWallChild> inner = new List<DrawingWallChild>();
                                _multiple[i].transform.DoRecursive((Transform t) =>
                                {
                                    if (t.TryGetComponent<DrawingWallChild>(out DrawingWallChild dwc))
                                    {
                                        inner.Add(dwc);
                                        _multipleChildrenMoreThanOne = true;
                                    }
                                });
                                _multiple_foldouts.Add(false);
                                _multipleChildren.Add(inner);
                            }

                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {

            if (MBSConfig.Singleton.pluginDisabled)
            {
                EditorGUILayout.HelpBox(HELPBOX_COMPONENTS_DISABLED, MessageType.Warning);
                return;
            }

            if (_builderNull)
            {
                EditorGUILayout.HelpBox(HELPBOX_BUILDER_MISSING, MessageType.Error);
                return;
            }

            _windowWidth = EditorGUIUtility.currentViewWidth - 25;

            boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.font = EditorStyles.boldFont;

            if (!_isMultiple)
            {
                if (_assetNull)
                {
                    EditorGUILayout.HelpBox(HELPBOX_ASSET_NULL, MessageType.Error);
                    return;
                }

                InspectorShowAssetsSelectionGrid();

                EditorGUILayout.Space(5, false);
                ShowAreasButtons();
                ShowChildrenModifiers();

            }
            else
            {
                if (!_sameBuilder)
                {
                    EditorGUILayout.HelpBox(HELPBOX_DIFFERENT_BUILDERS, MessageType.Error);
                    return;
                }

                if (_assetNull)
                {
                    EditorGUILayout.HelpBox(HELPBOX_ASSET_NULL, MessageType.Error);
                    return;
                }

                if (!_sameAsset)
                {
                    EditorGUILayout.HelpBox(HELPBOX_DIFFERENT_ASSETS, MessageType.Warning);
                    return;
                }

                InspectorShowAssetsSelectionGrid_MULTIPLE(_multiple, _originalAsset, _samePrefabIndex, _prefabIndex);
                ShowChildrenModifiers();
            }
        }


        private void ShowAreasButtons()
        {
            if (_single.AttachedAreas.Count > 0)
            {
                EditorGUILayout.LabelField("Areas");


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(5, false);
                EditorGUILayout.BeginVertical();

                for (int i = 0; i < _single.AttachedAreas.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(i + ". Area (area = " + _single.AttachedAreas[i].area.ToString() + ")");
                    if (GUILayout.Button("Select"))
                    {
                        _single.AttachedAreas[i].SelectAreaItems();
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5, false);
            }
        }

        private void InspectorShowAssetsSelectionGrid()
        {
            EditorGUILayout.LabelField("Asset Prefabs");

            EditorGUILayout.BeginHorizontal();
            {
                Rect itemPreviewRect = EditorGUILayout.GetControlRect(true, GUILayout.Width(128), GUILayout.Height(128));

                GameObject goToPreview;
                if (_single.CurrentPrefabIndex == -1)
                    goToPreview = _single.OriginalAsset.Prefabs.ElementAtOrDefault(0);
                else goToPreview = _single.OriginalAsset.Prefabs.ElementAtOrDefault(_single.CurrentPrefabIndex);

                EditorGUI.DrawPreviewTexture(itemPreviewRect, MBSEditorTools.GetAssetPreviewOrGray(goToPreview));

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(136));

                EditorGUI.BeginChangeCheck();
                {
                    int itemIndex = ModelSelectionGrid(_single.CurrentPrefabIndex, _single.OriginalAsset.Prefabs, 54);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _single.ChangeModel(itemIndex, false);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void InspectorShowAssetsSelectionGrid_MULTIPLE(List<DrawingWall> selectedWalls, MBSWallAsset asset, bool allUsingSamePrefab, int prefabIndex)
        {
            EditorGUILayout.LabelField("Asset Prefabs", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            {
                Rect itemPreviewRect = EditorGUILayout.GetControlRect(true, GUILayout.Width(128), GUILayout.Height(128));

                if (allUsingSamePrefab)
                {
                    GameObject goToPreview;
                    if (prefabIndex == -1)
                        goToPreview = asset.Prefabs.FirstOrDefault();
                    else goToPreview = asset.Prefabs.ElementAtOrDefault(prefabIndex);

                    EditorGUI.DrawPreviewTexture(itemPreviewRect, MBSEditorTools.GetAssetPreviewOrGray(goToPreview));
                }
                else
                    EditorGUI.DrawTextureAlpha(itemPreviewRect, Texture2D.grayTexture);

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(136));

                EditorGUI.BeginChangeCheck();
                {
                    int newPrefabIndex = ModelSelectionGrid(prefabIndex, asset.Prefabs, 54);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.SetCurrentGroupName(UNDO_MULTIPLE_PREFAB_CHANGING);
                        MBSBuilder builder = selectedWalls[0].Builder;


                        for (int i = 0; i < selectedWalls.Count; i++)
                        {
                            builder._drawingManager.RemoveDrawingItem(selectedWalls[i], false);
                            selectedWalls[i].RemoveAllAreas();
                            selectedWalls[i].ClearConnections();
                        }

                        List<DrawingWall> changedDWs = new List<DrawingWall>();
                        for (int i = 0; i < selectedWalls.Count; i++)
                        {
                            changedDWs.Add(selectedWalls[i].ChangeModel(newPrefabIndex, true));
                        }


                        List<DrawingWall> instantiated = new List<DrawingWall>();
                        for (int i = 0; i < changedDWs.Count; i++)
                        {
                            changedDWs[i].UpdateMesh();

                            foreach (DrawingWall c in changedDWs[i].FrontConnections)
                            {
                                if (!changedDWs.Contains(c) && !instantiated.Contains(c))
                                {
                                    c.UpdateMesh();
                                    instantiated.Add(c);
                                }
                            }

                            foreach (DrawingWall c in changedDWs[i].RearConnections)
                            {
                                if (!changedDWs.Contains(c) && !instantiated.Contains(c))
                                {
                                    c.UpdateMesh();
                                    instantiated.Add(c);
                                }
                            }
                        }

                        Selection.objects = changedDWs.Select(i => i.gameObject).ToArray();

                        for (int i = 0; i < selectedWalls.Count; i++)
                        {
                            selectedWalls[i].gameObject.DestroyImmediateUndo();
                        }
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }

        private int ModelSelectionGrid(int selected, GameObject[] models, int buttonSize)
        {
            GUILayout.BeginVertical("box", GUILayout.MinHeight(128));
            GUIStyle style = new GUIStyle("button");
            style.fixedWidth = buttonSize;
            style.fixedHeight = buttonSize;
            int retval = 0;

            if (models.Length != 0)
            {
                int columns = (int)(_windowWidth - 155) / buttonSize;
                int rows = (int)Mathf.Ceil((models.Length + columns - 1) / columns);
                Rect rect = GUILayoutUtility.GetAspectRect((float)columns / (float)rows);

                GUIContent[] thumbnails = new GUIContent[models.Length];
                for (int i = 0; i < thumbnails.Length; i++)
                {
                    Texture2D tex = AssetPreview.GetAssetPreview(Utilities.GetPreviewTexture(models[i]));
                    string text = models[i].name;
                    if (text == "") text = "Unnamed item";
                    thumbnails[i] = new GUIContent(tex, text);
                }


                retval = GUI.SelectionGrid(rect, System.Math.Min(selected, models.Length - 1), thumbnails, (int)columns, style);
            }
            else
            {
                EditorGUILayout.HelpBox(EBuilder_Inspector.ASSET_ITEMS_NOT_FOUND, MessageType.Error);
            }

            GUILayout.EndVertical();
            return retval;
        }

        private void SshowChildrenModifiers()
        {
            if (_singleChildren != null && _singleChildren.Count > 0)
            {
                _foldoutChildren = EditorGUILayout.Foldout(_foldoutChildren, "Children Modifcations", true);
                if (_foldoutChildren)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(5, false);
                    EditorGUILayout.BeginVertical();
                    EditorGUI.BeginChangeCheck();
                    for (int i = 0; i < _singleChildren.Count; i++)
                    {
                        _singleChildren[i]._doModify = EditorGUILayout.Toggle(_singleChildren[i].gameObject.name, _singleChildren[i]._doModify);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        _single.UpdateMesh();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void ShowChildrenModifiers()
        {
            if (!_isMultiple)
            {
                if (_singleChildren.Count > 0)
                {
                    _foldoutChildren = EditorGUILayout.Foldout(_foldoutChildren, "Children Modifcations", true, boldFoldout);

                    if (_foldoutChildren)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(5, false);
                        EditorGUILayout.BeginVertical();
                        EditorGUI.BeginChangeCheck();
                        for (int i = 0; i < _singleChildren.Count; i++)
                        {
                            _singleChildren[i]._doModify = EditorGUILayout.Toggle(_singleChildren[i].gameObject.name, _singleChildren[i]._doModify);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            _single.UpdateMesh();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                if (_multipleChildren.Count > 0 && _multipleChildrenMoreThanOne)
                {
                    _foldoutChildren = EditorGUILayout.Foldout(_foldoutChildren, "Children Modifcations", true, boldFoldout);

                    if (_foldoutChildren)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(10, false);

                        EditorGUILayout.BeginVertical();

                        for (int i = 0; i < _multiple.Count; i++)
                        {
                            if (_multipleChildren[i].Count > 0)
                            {
                                _multiple_foldouts[i] = EditorGUILayout.Foldout(_multiple_foldouts[i], _multiple[i].gameObject.name, true);

                                if (_multiple_foldouts[i])
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space(5, false);

                                    EditorGUILayout.BeginVertical();

                                    EditorGUI.BeginChangeCheck();
                                    {
                                        for (int j = 0; j < _multipleChildren[i].Count; j++)
                                        {
                                            _multipleChildren[i][j]._doModify = EditorGUILayout.Toggle(_multipleChildren[i][j].gameObject.name,
                                                                                                    _multipleChildren[i][j]._doModify);
                                        }
                                        if (EditorGUI.EndChangeCheck())
                                            _multiple[i].UpdateMesh();
                                    }

                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }

                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        private bool AreSelectedSameAsset(List<DrawingWall> walls)
        {
            bool sameGuid = true;
            string initialGuid = walls[0].OriginalAssetGUID;
            for (int i = 1; i < walls.Count; i++)
            {
                sameGuid &= (walls[i].OriginalAssetGUID == initialGuid);
            }
            return sameGuid;
        }

        private bool AreSelectedSamePrefab(List<DrawingWall> walls)
        {
            bool sameIndex = true;
            int initialIndex = walls[0].CurrentPrefabIndex;
            for (int i = 1; i < walls.Count; i++)
            {
                sameIndex &= (walls[i].CurrentPrefabIndex == initialIndex);
            }
            return sameIndex;
        }
    }
}
