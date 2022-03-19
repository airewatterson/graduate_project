using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace MBS
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DrawingWallChild))]
    public class EDrawingWallChild : Editor
    {
        private const string HELPBOX_COMPONENTS_DISABLED = "All MBS Components disabled. To enable do Tools -> MBS -> Enable All Components";
        private const string HELPBOX_BUILDER_MISSING = "MBSBuilder this wall(s) attached to is missing, can't work without it.";
        private const string MULTIPLE_ITEM_HAVE_NO_PARENT = "Some of the selected DrawingWallChilds have no DrawingWall component in parent gameobjects.";
        private const string MULTIPLE_PARENT_ASSET_NULL = "Some of the parents (DrawingWall) of selected DrawingWallChilds missed the reference to the original MBSAsset.";
        private const string SINGLE_PARENT_ASSET_NULL = "Can't reach original MBSAsset of selected component(s), can't work without it.";
        private const string HELPBOX_MULTIPLE_DIFFERENT_ASSETS = "Selected walls are using different assets, please select walls with same assets.";
        private const string UNDO_MULTIPLE_PREFAB_CHANGING = "MBS. Multiple Wall Prefab Changing";


        private DrawingWallChild _singleScript;
        private List<DrawingWallChild> _multipleScripts;

        private List<GameObject> _parents;

        private DrawingWall _dw;
        private List<DrawingWall> _dws;
        private List<bool> _dws_foldouts;

        private List<DrawingWallChild> _singleChildren;
        private List<List<DrawingWallChild>> _multipleChildren;

        private List<DrawingWallChild> _topChildren;

        private MBSWallAsset _orignalAsset;
        private int _prefabIndex;

        private bool _builderNull;
        private bool _parentNull;
        private bool _isMultiple;
        private bool _loopError;
        private bool _sameBuilder;
        private bool _sameAsset;
        private bool _samePrefab;
        private bool _assetNull;

        private float _windowWidth;

        private bool _showFoldoutChildren;
        private Vector2 scrollPos;

        private GUIStyle boldFoldout;
        private GUIStyle buttonStyle;
        private GUIStyle boldLabel;


        private void OnEnable()
        {
            if (targets.Length == 1) _isMultiple = false;
            else _isMultiple = true;

            if (!_isMultiple)
            {
                _singleScript = (DrawingWallChild)target;
                _singleChildren = new List<DrawingWallChild>();

                if (_singleScript.RootDW != null)
                {
                    _parentNull = false;
                    _dw = _singleScript.RootDW;
                    _builderNull = _dw.Builder == null;

                    _singleScript.RootDW.transform.DoRecursive((Transform t) =>
                    {
                        if (t.TryGetComponent<DrawingWallChild>(out DrawingWallChild dwc))
                        {
                            _singleChildren.Add(dwc);
                        }
                    });


                    if (!_builderNull && !_dw.Builder._awaken)
                        _dw.Builder.SoftInitialization();
                }
                else
                    _parentNull = true;
            }
            else
            if (_isMultiple)
            {
                _multipleScripts = targets.Cast<DrawingWallChild>().ToList();
                _multipleChildren = new List<List<DrawingWallChild>>();
                _dws = new List<DrawingWall>();
                _dws_foldouts = new List<bool>();

                for (int i = 0; i < targets.Length; i++)
                {
                    if (_multipleScripts[i].RootDW == null)
                    {
                        _loopError = true;
                        return;
                    }

                    _dws_foldouts.Add(false);
                    _dws.Add(_multipleScripts[i].RootDW);
                    List<DrawingWallChild> inner = new List<DrawingWallChild>();

                    if (_multipleScripts[i].RootDW.transform.childCount > 0)
                    {
                        _multipleScripts[i].RootDW.transform.DoRecursive((Transform t) =>
                        {
                            if (t.TryGetComponent<DrawingWallChild>(out DrawingWallChild child))
                            {
                                inner.Add(child);
                            }
                        });
                    }
                    _multipleChildren.Add(inner);
                }

                if (_dws.Count > 0 && !_loopError)
                {
                    MBSWallAsset initialAsset = _dws[0].OriginalAsset;
                    int initialPrefabIndex = _dws[0].CurrentPrefabIndex;

                    if (initialAsset == null)
                        _assetNull = true;

                    _orignalAsset = initialAsset;
                    _prefabIndex = initialPrefabIndex;

                    _builderNull = _dws.Any(i => i.Builder == null);
                    _sameAsset = _dws.All(i => i.OriginalAsset == initialAsset);
                    _assetNull = _dws.Any(i => i.OriginalAsset == null);
                    _sameBuilder = _dws.All(i => i.Builder == _dws[0].Builder);

                    if (!_builderNull && _sameBuilder && !_dws[0].Builder._awaken)
                        _dw.Builder.SoftInitialization();

                    if (_sameAsset)
                        _samePrefab = _dws.All(i => i.CurrentPrefabIndex == initialPrefabIndex);
                    else _samePrefab = false;
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

            boldFoldout = new GUIStyle("foldout");
            boldFoldout.font = EditorStyles.boldFont;
            buttonStyle = new GUIStyle("button");
            boldLabel = EditorStyles.boldLabel;



            if (!_isMultiple)
            {
                if (_parentNull)
                {
                    EditorGUILayout.HelpBox("Parent drawingWall null", MessageType.Error);
                    return;
                }
                if (_assetNull)
                {
                    EditorGUILayout.HelpBox(SINGLE_PARENT_ASSET_NULL, MessageType.Error);
                    return;
                }

                EditorGUILayout.Space(5);

                ParentSelectButton();

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Parent QuickAcces", boldLabel);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space(10, false);
                EditorGUILayout.BeginVertical();




                InspectorShowAssetsSelectionGrid();
                ShowAreasButtons();
                ShowChildrenModifiers();

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (_loopError)
                {
                    EditorGUILayout.HelpBox(MULTIPLE_ITEM_HAVE_NO_PARENT, MessageType.Error);
                    return;
                }
                if (_assetNull)
                {
                    EditorGUILayout.HelpBox(MULTIPLE_PARENT_ASSET_NULL, MessageType.Error);
                    return;
                }
                if (!_sameAsset)
                {
                    EditorGUILayout.HelpBox(HELPBOX_MULTIPLE_DIFFERENT_ASSETS, MessageType.Warning);
                    return;
                }

                EditorGUILayout.Space(5);

                ParentSelectButton();

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Parent QuickAcces", boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(10, false);
                EditorGUILayout.BeginVertical();

                InspectorShowAssetsSelectionGrid_MULTIPLE(_dws, _orignalAsset, _samePrefab, _prefabIndex);
                ShowChildrenModifiers();

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }


        private void ShowAreasButtons()
        {
            if (_dw.AttachedAreas.Count > 0)
            {
                EditorGUILayout.LabelField("Areas");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(10, false);

                EditorGUILayout.BeginVertical();

                for (int i = 0; i < _dw.AttachedAreas.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel((i + 1) + ". Area (area = " + _dw.AttachedAreas[i].area.ToString() + ")");

                    if (GUILayout.Button("Select"))
                    {
                        _dw.AttachedAreas[i].SelectAreaItems();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10, false);
            }
        }

        private void InspectorShowAssetsSelectionGrid()
        {
            EditorGUILayout.LabelField("Asset Prefabs");

            EditorGUILayout.BeginHorizontal();
            {
                Rect itemPreviewRect = EditorGUILayout.GetControlRect(true, GUILayout.Width(128), GUILayout.Height(128));

                GameObject goToPreview;
                if (_dw.CurrentPrefabIndex == -1)
                    goToPreview = _dw.OriginalAsset.Prefabs.ElementAtOrDefault(0);
                else goToPreview = _dw.OriginalAsset.Prefabs.ElementAtOrDefault(_dw.CurrentPrefabIndex);

                EditorGUI.DrawPreviewTexture(itemPreviewRect, MBSEditorTools.GetAssetPreviewOrGray(goToPreview));

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(136));

                EditorGUI.BeginChangeCheck();
                {
                    int itemIndex = ModelSelectionGrid(_dw.CurrentPrefabIndex, _dw.OriginalAsset.Prefabs, 54);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _dw.ChangeModel(itemIndex, false);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void InspectorShowAssetsSelectionGrid_MULTIPLE(List<DrawingWall> selectedWalls, MBSWallAsset asset, bool allUsingSamePrefab, int prefabIndex)
        {
            EditorGUILayout.LabelField("Asset Prefabs", boldLabel);

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

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(136));

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

        private void ShowChildrenModifiers()
        {
            if (!_isMultiple)
            {
                if (_singleChildren.Count > 0)
                {
                    _showFoldoutChildren = EditorGUILayout.Foldout(_showFoldoutChildren, "Children Modifcations", true);

                    if (_showFoldoutChildren)
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
                            _dw.UpdateMesh();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                if (_multipleChildren.Count > 0)
                {
                    _showFoldoutChildren = EditorGUILayout.Foldout(_showFoldoutChildren, "Children Modifcations", true);

                    if (_showFoldoutChildren)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(10, false);

                        EditorGUILayout.BeginVertical();

                        for (int i = 0; i < _dws.Count; i++)
                        {
                            if (_multipleChildren[i].Count > 0)
                            {
                                _dws_foldouts[i] = EditorGUILayout.Foldout(_dws_foldouts[i], _dws[i].gameObject.name, true);
                                if (_dws_foldouts[i])
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
                                            _dws[i].UpdateMesh();
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

        private void ParentSelectButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Parent DrawingWall", buttonStyle, boldLabel);
            if (GUILayout.Button("Select"))
            {
                if (targets.Length > 1)
                {
                    _parents = new List<GameObject>();
                    for (int i = 0; i < _multipleScripts.Count; i++)
                    {
                        if (_multipleScripts[i].RootDW == null)
                        {
                            _multipleScripts[i].RootDW = _multipleScripts[i].transform.GetComponentInParent<DrawingWall>();
                            if (_singleScript.RootDW == null)
                            {
                                Debug.LogError("Can't select parent DrawingWall object, it is missing.");
                                continue;
                            }
                        }
                        _parents.Add(_multipleScripts[i].RootDW.gameObject);
                    }
                    Selection.objects = _parents.ToArray();
                }
                else
                {
                    if (_singleScript.RootDW == null)
                    {
                        _singleScript.RootDW = _singleScript.transform.GetComponentInParent<DrawingWall>();
                        if (_singleScript.RootDW == null)
                        {
                            Debug.LogError("Can't select parent DrawingWall object, it is missing.");
                            return;
                        }
                    }
                    _singleScript.RootDW.gameObject.SelectObject();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}