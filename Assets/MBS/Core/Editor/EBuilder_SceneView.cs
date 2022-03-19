using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace MBS
{
    internal class EBuilder_SceneView
    {
        private const string CANT_START_DRAWING_FIRSTPREFAB_NULL = "MBS. Can't start drawing, asset does not have any prefabs.";
        private const string CANT_START_DRAWING_ASSET_NULL = "MBS. Can't start drawing, no asset selected, or asset is missing.";
        private Event evnt;
        private int wallName_iterator;


        private MBSBuilder bldr;
        private AssetsData ad;
        private SceneData sd;

        private bool leftMouseButtonDown;

        internal void SceneViewBootstrap(MBSBuilder builder)
        {
            bldr = builder;
            sd = builder._sceneData;
            ad = builder._assetsData;

            Tools.current = Tool.None;
            evnt = Event.current;
            sd.evnt = Event.current;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));

            MousePositionObserver();

            string escText = "<b>ESC</b> - ";
            string mode = "<b>MBS: ";

            switch (sd.sceneMode)
            {
                case SceneViewMode.Idle:
                    MousePositioning();
                    IdleMode();
                    escText += "deselect MBSBuilder object;";
                    mode += "Idle Mode";
                    break;
                case SceneViewMode.Drawing:
                    MousePositioning();
                    DrawingMode();
                    escText += "exit Drawing mode;";
                    mode += "Drawing Mode";
                    break;
                case SceneViewMode.ObjectPicker:
                    ObjectPicker();
                    escText += "exit Object picker mode;";
                    mode += "Object Picker Mode";
                    break;
                case SceneViewMode.PositionPicker:
                    PositionPicker();
                    escText += "exit Position picker mode;";
                    mode += "Position Picker Mode";
                    break;

                default: break;
            }

            Handles.BeginGUI();

            Vector2 boxPos1 = new Vector2(0, Screen.height - 65);
            Vector2 boxSize1 = new Vector2(Screen.width / 5, 25);

            Vector2 boxPos2 = new Vector2(Screen.width / 5, Screen.height - 65);
            Vector2 boxSize2 = new Vector2((Screen.width / 5) * 4, 25);

            string snapOnOff = (sd.doSnapToEndPoints) ? "ON" : "OFF";
            string ctrl_text = "<b>CTRL</b> - Snapping to the walls endpoints " + "(" + snapOnOff + ");";
            string g_text = "<b>G</b> - Set the grid origin point";
            string tool_text = (ad.Current_Tool == BuilderTools.Floors) ? "<b>SHIFT</b> - Autofill closed area;" : "";

            GUIStyle s = new GUIStyle("box");
            s.alignment = TextAnchor.MiddleLeft;
            s.richText = true;
            s.normal.background = null;
            GUI.color = Color.white;
            GUI.backgroundColor = new Color(0, 0, 0, 3);
            GUI.Box(new Rect(boxPos1, boxSize1), mode + "</b>", s);
            GUI.backgroundColor = new Color(0, 0, 0, 2.5f);

            string text = ctrl_text + "     " +
                        g_text + "     " +
                        escText + "     " +
                        tool_text;

            GUI.Box(new Rect(boxPos2, boxSize2), text, s);
            Handles.EndGUI();
        }

        private void MousePositionObserver()
        {
            if (evnt.type == EventType.MouseEnterWindow)
            {
                sd.pointerEnabled = true;
                SceneView.currentDrawingSceneView.Focus();
                return;
            }
            else if (evnt.type == EventType.MouseLeaveWindow)
            {
                sd.pointerEnabled = false;
                return;
            }
        }
        private void MousePositioning()
        {
            if (sd.pointerEnabled)
            {
                if (evnt.type == EventType.MouseMove)
                {
                    var pointerPos = PointerLocalPos(bldr.GizmoPos);
                    var posNonGrid = pointerPos.ElementAt(0);
                    var posGrid = pointerPos.ElementAt(1);

                    sd.mCurrentPosNG = posNonGrid.position;
                    sd.mCurrentPosG = posGrid.position;

                    if (posGrid.snapped)
                        bldr.SetGizmoColorSnapped();
                    else bldr.SetGizmoColorUnsnaped();

                    bldr.GizmoPos = bldr.WPos(sd.mCurrentPosG);

                    HandleUtility.Repaint();
                }
            }
        }
        private void GridPosition_G()
        {
            if (evnt.shift || evnt.alt || evnt.control) return;

            if (evnt.type == EventType.KeyDown && evnt.keyCode == KeyCode.G)
            {
                bldr.GridMan_SetParams(gridPos: sd.mCurrentPosG);
            }
        }

        private void IdleMode()
        {
            if (ad.Current_Tool == BuilderTools.Floors)
            {
                ShiftButtonListener();

                if (FloorGizmoSupervisor(out FloorTileType floorType, out Vector3 rotation))
                {
                    bldr.SetFloorGizmoMesh(floorType);
                    bldr.GizmoRot = rotation;
                }
                else
                {
                    bldr.SetFloorGizmoMesh();
                }
            }
            OnKeyDownListener(KeyCode.G, () =>
            {
                bldr.GridMan_SetParams(gridPos: sd.mCurrentPosG);
            }, new[] { KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt });
            OnKeyUpListener(KeyCode.LeftControl, () =>
            {
                sd.doSnapToEndPoints = !sd.doSnapToEndPoints;
            });

            MouseButtonsListener(() =>
            {
                if (ad.Current_Asset == null)
                {
                    Debug.LogError(CANT_START_DRAWING_ASSET_NULL);
                    return;
                }

                if (ad.Current_Asset_FirstPrefab() == null)
                {
                    Debug.LogWarning(CANT_START_DRAWING_FIRSTPREFAB_NULL);
                    return;
                }

                SetPointerStartPosition();
                BeginDrawing();
                sd.SetDrawingMode();
            });
            OnKeyUpListener(KeyCode.Escape, () =>
            {
                Selection.activeGameObject = null;
                return;
            }, new[] { KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt });
        }
        private void DrawingMode()
        {
            switch (ad.Current_Tool)
            {
                case BuilderTools.Walls:
                    DrawWallItems();
                    break;
                case BuilderTools.Floors:
                    DrawFloorItems();
                    break;
            }
            GridPosition_G();
            MouseButtonsListener(() =>
            {
                EndDrawing();
                ClearDrawingVariables();
                sd.SetIdleMode();
            });
            OnKeyUpListener(KeyCode.Escape, () =>
            {
                sd.SetIdleMode();
                RemoveDrawedObjects();
                ClearDrawingVariables();
            }, new[] { KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt });
        }
        private void ObjectPicker()
        {
            switch (sd.builderParameter)
            {
                case BuilderParameters.GridSize:
                    ObjectPicker_GridSize();
                    break;
                case BuilderParameters.LevelHeight:
                    ObjectPicker_LevelHeight();
                    break;

                default: break;
            }
            OnKeyUpListener(KeyCode.Escape, () =>
            {
                sd.SetIdleMode();
            }, new[] { KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt });
        }
        private void PositionPicker()
        {
            if (sd.pointerEnabled)
            {
                var raycastHit = RaycastPosition();

                sd.pickerPos = raycastHit.pos;
                if (raycastHit.isTopOrBot)
                    bldr.SetGizmoColorSnapped();
                else bldr.SetGizmoColorUnsnaped();

                Handles.BeginGUI();
                Vector2 boxPos = evnt.mousePosition + new Vector2(20, 0);
                Vector2 boxSize = new Vector2(150, 45);
                Vector2 labelPos = boxPos + new Vector2(10, 18);
                Vector2 labelSize = new Vector2(70, 22);
                GUI.Box(new Rect(boxPos, boxSize), "Height: " + sd.pickerPos.y, EditorStyles.toolbarButton);
                Handles.EndGUI();

                MouseButtonsListener(() =>
                {
                    ad.LevelHeight = sd.pickerPos.y;
                    bldr.GridMan_SetParams(ad);
                    sd.SetIdleMode();

                });

                HandleUtility.Repaint();
            }

            OnKeyUpListener(KeyCode.Escape, () =>
            {
                sd.SetIdleMode();
            }, new[] { KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt });
        }

        private void ObjectPicker_LevelHeight()
        {
            if (sd.pointerEnabled)
            {
                var raycastHit = GetRaycastObjectParameter(RaycastObjectParameter.ObjectHeight);

                if (raycastHit.gameObject != null)
                {
                    sd.ObjPicker_Object = raycastHit.gameObject;
                    sd.ObjPicker_ObjCenterLocal = raycastHit.objParams.secondParam.center;
                    sd.ObjPicker_ObjExtents = raycastHit.objParams.secondParam.extents;

                    Handles.BeginGUI();
                    Vector2 boxPos = evnt.mousePosition + new Vector2(20, 0);
                    Vector2 boxSize = new Vector2(150, 45);
                    Vector2 labelPos = boxPos + new Vector2(10, 18);
                    Vector2 labelSize = new Vector2(70, 22);
                    GUI.Box(new Rect(boxPos, boxSize), "Object Height: " + raycastHit.objParams.mainParam, EditorStyles.toolbarButton);
                    Handles.EndGUI();
                }
                else
                {
                    sd.ObjPicker_Object = null;
                    sd.ObjPicker_ObjCenterLocal = Vector3.zero;
                    sd.ObjPicker_ObjExtents = Vector3.zero;


                    Handles.BeginGUI();
                    Vector2 boxPos = evnt.mousePosition + new Vector2(20, 0);
                    Vector2 boxSize = new Vector2(150, 45);
                    Vector2 labelPos = boxPos + new Vector2(10, 18);
                    Vector2 labelSize = new Vector2(70, 22);
                    GUI.Box(new Rect(boxPos, boxSize), "No objects selected", EditorStyles.toolbarButton);
                    Handles.EndGUI();
                }

                MouseButtonsListener(() =>
                {
                    bldr.GridMan_SetParams(levelHeight: raycastHit.objParams.mainParam);
                    sd.SetIdleMode();
                });

                HandleUtility.Repaint();
            }
            else
            {
                MouseButtonsListener(() =>
                {
                    sd.SetIdleMode();
                });
            }
        }
        private void ObjectPicker_GridSize()
        {
            if (sd.pointerEnabled)
            {
                var raycastHit = GetRaycastObjectParameter(RaycastObjectParameter.XAxisSize);

                if (raycastHit.gameObject != null)
                {
                    sd.ObjPicker_Object = raycastHit.gameObject;
                    sd.ObjPicker_ObjCenterLocal = raycastHit.objParams.secondParam.center;
                    sd.ObjPicker_ObjExtents = raycastHit.objParams.secondParam.extents;

                    Handles.BeginGUI();
                    Vector2 boxPos = evnt.mousePosition + new Vector2(20, 0);
                    Vector2 boxSize = new Vector2(150, 45);
                    Vector2 labelPos = boxPos + new Vector2(10, 18);
                    Vector2 labelSize = new Vector2(70, 22);
                    GUI.Box(new Rect(boxPos, boxSize), "Object Height: " + raycastHit.objParams.mainParam, EditorStyles.toolbarButton);
                    Handles.EndGUI();
                }
                else
                {
                    sd.ObjPicker_Object = null;
                    sd.ObjPicker_ObjCenterLocal = Vector3.zero;
                    sd.ObjPicker_ObjExtents = Vector3.zero;

                    Handles.BeginGUI();
                    Vector2 boxPos = evnt.mousePosition + new Vector2(20, 0);
                    Vector2 boxSize = new Vector2(150, 45);
                    Vector2 labelPos = boxPos + new Vector2(10, 18);
                    Vector2 labelSize = new Vector2(70, 22);
                    GUI.Box(new Rect(boxPos, boxSize), "No objects selected", EditorStyles.toolbarButton);
                    Handles.EndGUI();
                }

                MouseButtonsListener(() =>
                {
                    bldr.GridMan_SetParams(gridSize: raycastHit.objParams.mainParam);
                    sd.SetIdleMode();
                });

                HandleUtility.Repaint();
            }
            else
            {
                MouseButtonsListener(() =>
                {
                    sd.SetIdleMode();
                });
            }
        }

        private (Vector3 pos, bool isTopOrBot) RaycastPosition()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(evnt.mousePosition);
            Vector3 hitPointWorld = default;
            Vector3 hitPointLocal = default;
            bool isTopOrBot = false;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                hitPointWorld = hit.point;

                Transform tr = hit.collider.transform;
                hitPointLocal = tr.InverseTransformPoint(hit.point);
                if (tr.TryGetComponent<MeshFilter>(out MeshFilter mf))
                {
                    Vector3 center = mf.sharedMesh.bounds.center.RoundDecimals();
                    Vector3 extents = mf.sharedMesh.bounds.extents.RoundDecimals();
                    Vector3 diff = (hitPointLocal - center).RoundDecimals();
                    if (Mathf.Abs(diff.y) == extents.y)
                        isTopOrBot = true;
                }
            }
            return (hitPointWorld.RoundDecimals(), isTopOrBot);
        }
        private (GameObject gameObject, (float mainParam, (Vector3 center, Vector3 extents) secondParam) objParams) GetRaycastObjectParameter(RaycastObjectParameter objParameter)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(evnt.mousePosition);
            Vector3 hitPointWorld = default;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                hitPointWorld = hit.point;

                Transform tr = hit.collider.transform;
                if (tr.TryGetComponent<MeshFilter>(out MeshFilter mf))
                {
                    Vector3 objSize = Vector3.zero;
                    Vector3 objCenter = Vector3.zero;
                    Vector3 objExtents = Vector3.zero;

                    if (tr.TryGetComponent<DrawingWall>(out DrawingWall dw))
                    {
                        objSize = dw.AssetSize;
                        objCenter = Vector3.zero + Vector3.up * (objSize.y / 2);
                        objExtents = objSize / 2;
                    }
                    else
                    {
                        objSize = mf.sharedMesh.bounds.size;
                        objCenter = mf.sharedMesh.bounds.center;
                        objExtents = mf.sharedMesh.bounds.extents;
                    }
                    switch (objParameter)
                    {
                        case RaycastObjectParameter.ObjectHeight:
                            return (tr.gameObject, (objSize.y.RoundDecimals(), (objCenter, objExtents)));
                        case RaycastObjectParameter.XAxisSize:
                            return (tr.gameObject, (objSize.x.RoundDecimals(), (objCenter, objExtents)));
                    }

                }
            }
            return (null, (0, (Vector3.zero, Vector3.zero)));
        }

        private void EscapeButtonListener(Action onEscapeButton)
        {
            if (evnt.type == EventType.KeyDown && evnt.keyCode == KeyCode.Escape)
            {
                onEscapeButton.Invoke();
            }
        }

        private void OnKeyDownListener(KeyCode key, Action onKeyDownAction, params KeyCode[] excludeKeys)
        {
            if (evnt.type == EventType.KeyDown)
            {
                bool isAnyAdditionalKeyPressed = false;

                if (excludeKeys.Length > 0)
                {
                    for (int i = 0; i < excludeKeys.Length; i++)
                    {
                        if (excludeKeys[i] == KeyCode.LeftControl || excludeKeys[i] == KeyCode.RightControl)
                        {
                            isAnyAdditionalKeyPressed |= evnt.control;
                        }
                        else if (excludeKeys[i] == KeyCode.LeftShift || excludeKeys[i] == KeyCode.RightShift)
                        {
                            isAnyAdditionalKeyPressed |= evnt.shift;
                        }
                        else if (excludeKeys[i] == KeyCode.LeftAlt || excludeKeys[i] == KeyCode.RightAlt)
                        {
                            isAnyAdditionalKeyPressed |= evnt.shift;
                        }
                    }
                }

                if (evnt.keyCode == key && !isAnyAdditionalKeyPressed)
                {
                    onKeyDownAction.Invoke();
                }
            }
        }

        private void OnKeyUpListener(KeyCode key, Action onKeyUpAction, params KeyCode[] excludeKeys)
        {
            if (evnt.type == EventType.KeyUp)
            {
                bool isAnyAdditionalKeyPressed = false;

                if (excludeKeys.Length > 0)
                {
                    for (int i = 0; i < excludeKeys.Length; i++)
                    {
                        if (excludeKeys[i] == KeyCode.LeftControl || excludeKeys[i] == KeyCode.RightControl)
                        {
                            isAnyAdditionalKeyPressed |= evnt.control;
                        }
                        else if (excludeKeys[i] == KeyCode.LeftShift || excludeKeys[i] == KeyCode.RightShift)
                        {
                            isAnyAdditionalKeyPressed |= evnt.shift;
                        }
                        else if (excludeKeys[i] == KeyCode.LeftAlt || excludeKeys[i] == KeyCode.RightAlt)
                        {
                            isAnyAdditionalKeyPressed |= evnt.shift;
                        }
                    }
                }
                if (evnt.keyCode == key && !isAnyAdditionalKeyPressed)
                {
                    onKeyUpAction.Invoke();
                }
            }
        }


        private void MouseButtonsListener(Action onLeftButtonAction)
        {
            if (evnt.alt) return;

            if (evnt.type == EventType.MouseDown)
            {

                if (evnt.button == 0 && !leftMouseButtonDown)
                {
                    leftMouseButtonDown = true;
                    onLeftButtonAction.Invoke();
                }
                if (evnt.button == 1)
                {
                    sd.pointerEnabled = false;
                }
            }
            else if (evnt.type == EventType.MouseUp)
            {
                if (evnt.button == 0)
                {
                    leftMouseButtonDown = false;
                }
                if (evnt.button == 1)
                {
                    sd.pointerEnabled = true;
                }
            }

        }
        private void ShiftButtonListener()
        {
            if (evnt.alt || evnt.control || !sd.pointerEnabled) return;

            if (evnt.type == EventType.KeyUp && evnt.keyCode == KeyCode.LeftShift)
            {
                if (sd.pointerEnabled)
                {
                    if (sd.mPrevPosG != sd.mCurrentPosG)
                    {
                        Area areaAtPoint = bldr._areaManager.GetAreaAtPoint(sd.mCurrentPosNG);
                        if (areaAtPoint != null)
                        {
                            SetPointerStartPosition();
                            AutoFillFloor();
                        }
                    }
                    sd.mPrevPosG = sd.mCurrentPosG;
                }
            }
        }

        private IEnumerable<(Vector3 position, bool snapped)> PointerLocalPos(Vector3 prevPos)
        {
            SceneData sd = bldr._sceneData;
            AssetsData ad = bldr._assetsData;

            bool isSnapped = false;

            float pointerHeight = ad.CalcLevelHeight;

            float gridSize = ad.GridSize;

            Vector3 assetSize = ad.Current_Asset_Size;
            BuilderTools curTool = ad.Current_Tool;

            Vector3 mousePos = evnt.mousePosition;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePos);
            Plane plane = new Plane(Vector3.up, new Vector3(0, pointerHeight, 0));

            if (plane.Raycast(worldRay, out var distance))
                mousePos = worldRay.GetPoint(distance);
            else
                mousePos = Vector3.zero;

            Vector3 localPos = bldr.LPos(mousePos);

            yield return (localPos, isSnapped);

            Vector3 snappedPos = localPos;

            if (!sd.isDrawingMode)
            {
                switch (curTool)
                {
                    case BuilderTools.Walls:
                        snappedPos = new Vector3(
                            Mathf.Round((localPos.x - ad.GridPosition.x) / gridSize) * gridSize + ad.GridPosition.x,
                            0,
                            Mathf.Round((localPos.z - ad.GridPosition.z) / gridSize) * gridSize + ad.GridPosition.z
                        );
                        if (sd.doSnapToEndPoints)
                        {
                            if (bldr._drawingManager.TryFindEndPointToSnap(localPos, snappedPos, 0.5f, out Vector3 nearestPoint))
                            {
                                snappedPos = nearestPoint;
                                isSnapped = true;
                            }
                        }
                        break;

                    case BuilderTools.Floors:

                        float x = (assetSize.x / 2).RoundDecimals();
                        float z = (assetSize.z / 2).RoundDecimals();
                        if (sd.doSnapToEndPoints)
                        {
                            Vector3 topRight, botRight, topLeft, botLeft;
                            topRight = localPos + (new Vector3(x, 0, z));
                            botRight = localPos + (new Vector3(x, 0, -z));
                            botLeft = localPos + (new Vector3(-x, 0, -z));
                            topLeft = localPos + (new Vector3(-x, 0, z));

                            Vector3 tr, br, bl, tl;
                            bool isTopRight, isBotRight, isBotLeft, isTopLeft;
                            isTopRight = bldr._drawingManager.TryFindEndPointToSnap(topRight, 0.5f, out tr);
                            isBotRight = bldr._drawingManager.TryFindEndPointToSnap(botRight, 0.45f, out br);
                            isBotLeft = bldr._drawingManager.TryFindEndPointToSnap(botLeft, 0.5f, out bl);
                            isTopLeft = bldr._drawingManager.TryFindEndPointToSnap(topLeft, 0.45f, out tl);

                            if (isTopRight || isBotRight || isBotLeft || isTopLeft)
                            {
                                Vector3 maxD = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                                Vector3 tr_dif = (isTopRight) ? tr - topRight : maxD;
                                Vector3 br_dif = (isBotRight) ? br - botRight : maxD;
                                Vector3 bl_dif = (isBotLeft) ? bl - botLeft : maxD;
                                Vector3 tl_dif = (isTopLeft) ? tl - topLeft : maxD;

                                Vector3 a = (tr_dif.magnitude <= br_dif.magnitude) ? tr_dif : br_dif;
                                Vector3 b = (bl_dif.magnitude <= tl_dif.magnitude) ? bl_dif : tl_dif;
                                Vector3 c = (a.magnitude <= b.magnitude) ? a : b;

                                snappedPos += c;
                                isSnapped = true;
                            }
                            else
                            {
                                float halfGrid = (gridSize / 2).RoundDecimals();
                                snappedPos = new Vector3(
                                    Mathf.Floor((localPos.x - ad.GridPosition.x) / gridSize) * gridSize + halfGrid + ad.GridPosition.x,
                                    0,
                                    Mathf.Floor((localPos.z - ad.GridPosition.z) / gridSize) * gridSize + halfGrid + ad.GridPosition.z
                                );
                            }
                        }
                        else
                        {
                            float halfGrid = (gridSize / 2).RoundDecimals();
                            snappedPos = new Vector3(
                                Mathf.Floor((localPos.x - ad.GridPosition.x) / gridSize) * gridSize + halfGrid + ad.GridPosition.x,
                                0,
                                Mathf.Floor((localPos.z - ad.GridPosition.z) / gridSize) * gridSize + halfGrid + ad.GridPosition.z
                            );
                        }
                        break;
                }
            }
            else
            {
                Vector3 diff = localPos - sd.mStartPosG;

                switch (curTool)
                {
                    case BuilderTools.Walls:

                        float absDifX = Mathf.Abs(diff.x);
                        float absDifZ = Mathf.Abs(diff.z);

                        if (absDifX != absDifZ)
                        {
                            if (absDifX == Mathf.Min(absDifX, absDifZ))
                            {
                                if (absDifX != 0 && absDifZ != 0)
                                {
                                    if (absDifX > absDifZ / 2) localPos.x = sd.mStartPosG.x + absDifZ * Mathf.Sign(diff.x);
                                    else localPos.x = sd.mStartPosG.x;
                                }
                            }
                            else
                            {
                                if (absDifX != 0 && absDifZ != 0)
                                {
                                    if (absDifZ > absDifX / 2) localPos.z = sd.mStartPosG.z + absDifX * Mathf.Sign(diff.z);
                                    else localPos.z = sd.mStartPosG.z;
                                }
                            }
                        }

                        diff = localPos - sd.mStartPosG;

                        snappedPos = new Vector3(
                            sd.mStartPosG.x + Mathf.Round((diff.x) / ad.Current_Asset_Size.x) * ad.Current_Asset_Size.x,
                            0,
                            sd.mStartPosG.z + Mathf.Round((diff.z) / ad.Current_Asset_Size.x) * ad.Current_Asset_Size.x
                        );
                        break;

                    case BuilderTools.Floors:

                        float halfGrid = (gridSize / 2).RoundDecimals();

                        snappedPos = new Vector3(
                            Mathf.Floor((localPos.x - ad.GridPosition.x) / gridSize) * gridSize + halfGrid + ad.GridPosition.x,
                            0,
                            Mathf.Floor((localPos.z - ad.GridPosition.z) / gridSize) * gridSize + halfGrid + ad.GridPosition.z
                        );
                        break;
                }
            }

            snappedPos.y = pointerHeight;
            yield return (snappedPos, isSnapped);
        }

        private bool FloorGizmoSupervisor(out FloorTileType outFloorType, out Vector3 outRotation)
        {
            SceneData sd = bldr._sceneData;
            AssetsData ad = bldr._assetsData;

            outFloorType = FloorTileType.None;
            outRotation = default;

            if (ad.Current_Asset == null || ad.Current_Asset_MeshFilter(FloorTileType.Square) == null || ad.Current_Asset_MeshFilter(FloorTileType.Corner) == null)
                return false;

            Vector3 rightTop, rightBot, leftTop, leftBot;
            rightTop = sd.mCurrentPosG + (new Vector3(ad.Current_Asset_Size.x, 0, ad.Current_Asset_Size.z) / 2);
            rightBot = sd.mCurrentPosG + (new Vector3(ad.Current_Asset_Size.x, 0, -ad.Current_Asset_Size.z) / 2);
            leftTop = sd.mCurrentPosG + (new Vector3(-ad.Current_Asset_Size.x, 0, ad.Current_Asset_Size.z) / 2);
            leftBot = sd.mCurrentPosG + (new Vector3(-ad.Current_Asset_Size.x, 0, -ad.Current_Asset_Size.z) / 2);

            bool d1 = bldr._drawingManager.IsThereWallOnLine(rightTop, leftBot);
            bool d2 = bldr._drawingManager.IsThereWallOnLine(leftTop, rightBot);

            if (d1 && !d2)
            {
                float dist1 = Vector3.Distance(sd.mCurrentPosNG, rightBot);
                float dist2 = Vector3.Distance(sd.mCurrentPosNG, leftTop);

                outFloorType = FloorTileType.Corner;

                if (dist1 < dist2)
                {
                    outRotation = new Vector3(0, 90, 0);
                    return true;
                }
                else if (dist1 > dist2)
                {
                    outRotation = new Vector3(0, -90, 0);
                    return true;
                }
            }
            else if (d2 && !d1)
            {
                float dist1 = Vector3.Distance(sd.mCurrentPosNG, rightTop);
                float dist2 = Vector3.Distance(sd.mCurrentPosNG, leftBot);

                outFloorType = FloorTileType.Corner;

                if (dist1 < dist2)
                {
                    outRotation = new Vector3(0, 0, 0);
                    return true;
                }
                else if (dist1 > dist2)
                {
                    outRotation = new Vector3(0, 180, 0);
                    return true;
                }
            }
            return false;
        }

        private void AutoFillFloor()
        {
            BeginDrawing();
            bldr._drawingManager.AutoFillFloor(sd, ad);
            EndDrawing();
        }
        private void BeginDrawing()
        {
            switch (ad.Current_Tool)
            {
                case BuilderTools.Walls:
                    bldr._drawingManager.BeginDrawingWall(sd, ad);
                    break;
                case BuilderTools.Floors:
                    bldr._drawingManager.BeginDrawingFloor(sd);
                    bldr._drawingManager.BeginDrawingFloorLine(0, sd);
                    break;
            }
        }
        private void EndDrawing()
        {
            switch (ad.Current_Tool)
            {
                case BuilderTools.Floors:
                    if (evnt.shift)
                        bldr._drawingManager.EndDrawingFloor(sd, "Auto Fill Floor");
                    else
                        bldr._drawingManager.EndDrawingFloor(sd);
                    break;

                case BuilderTools.Walls:
                    bldr._drawingManager.EndDrawingWall(sd, ad);
                    break;
            }
        }

        private void DrawWallItems()
        {
            Vector3 deltaPos = (sd.mCurrentPosG - sd.mStartPosG).RoundDecimals();


            int itemsNumber = 0;

            if (Mathf.Abs(deltaPos.x) == Mathf.Abs(deltaPos.z))
            {
                itemsNumber = Mathf.RoundToInt(deltaPos.magnitude / (ad.Current_Asset_Size.x / Mathf.Sin(Mathf.Deg2Rad * 45)));
                sd.drawingAt45Deg = true;
            }
            else
            {
                itemsNumber = Mathf.RoundToInt(deltaPos.magnitude / ad.Current_Asset_Size.x);
                sd.drawingAt45Deg = false;
            }

            if (itemsNumber > sd.drawingWallItems.Count)
            {
                for (int i = sd.drawingWallItems.Count; i < itemsNumber; i++)
                {
                    GameObject prefab = ad.Current_Asset_FirstPrefab();
                    if (prefab != null)
                    {
                        prefab = GameObject.Instantiate(prefab);
                        prefab.transform.SetParent(sd.drawingGroupRoot.transform, false);
                        prefab.transform.name = wallName_iterator.ToString();
                        sd.drawingWallItems.Add(prefab);
                        wallName_iterator++;
                    }
                }
            }
            else if (itemsNumber < sd.drawingWallItems.Count)
            {
                for (int i = sd.drawingWallItems.Count - 1; i >= itemsNumber; i--)
                {
                    GameObject.DestroyImmediate(sd.drawingWallItems[i]);
                    sd.drawingWallItems.RemoveAt(i);
                    wallName_iterator--;
                }
            }

            if (sd.drawingAt45Deg)
            {
                for (int i = 0; i < sd.drawingWallItems.Count; i++)
                {
                    sd.drawingWallItems[i].transform.localPosition = (Vector3.right * ((ad.Current_Asset_Size.x / Mathf.Sin(Mathf.Deg2Rad * 45) * i)
                                                                   + (ad.Current_Asset_Size.x / Mathf.Sin(Mathf.Deg2Rad * 45)) / 2)) + (sd.drawingItemOffset * i);
                    sd.drawingWallItems[i].transform.localScale = new Vector3(1 / Mathf.Sin(Mathf.Deg2Rad * 45),
                                                                              sd.drawingWallItems[i].transform.localScale.y,
                                                                              sd.drawingWallItems[i].transform.localScale.z);
                }
            }
            else
            {
                for (int i = 0; i < sd.drawingWallItems.Count; i++)
                {
                    sd.drawingWallItems[i].transform.localPosition = (Vector3.right * (ad.Current_Asset_Size.x * i + (ad.Current_Asset_Size.x / 2)))
                                                                   + (sd.drawingItemOffset * i);
                    sd.drawingWallItems[i].transform.localScale = new Vector3(1, sd.drawingWallItems[i].transform.localScale.y,
                                                                               sd.drawingWallItems[i].transform.localScale.z);
                }
            }

            sd.drawingGroupRoot.transform.right = bldr.transform.TransformDirection(deltaPos.normalized);
        }
        private void DrawFloorItems()
        {
            Vector3 deltaPos = sd.mCurrentPosNG - sd.mStartPosG;
            int xLength = Mathf.Abs(Mathf.RoundToInt(deltaPos.x / ad.GridSize)) + 1;
            int zLength = Mathf.Abs(Mathf.RoundToInt(deltaPos.z / ad.GridSize)) + 1;

            int lineNumber = sd.drawingFloorLinesItems.Count;
            int itemNumber = sd.drawingFloorLinesItems[0].Count;

            if (xLength > lineNumber || zLength > itemNumber)
            {
                for (int x = 0; x < xLength; x++)
                {
                    Vector3 localLinePos = sd.mStartPosG + (Vector3.right * x * ad.GridSize * Mathf.Sign(deltaPos.x));

                    if (x >= lineNumber)
                    {
                        bldr._drawingManager.BeginDrawingFloorLine(x + 1, sd);
                    }

                    itemNumber = sd.drawingFloorLinesItems[x].Count;

                    for (int z = itemNumber; z < zLength; z++)
                    {
                        Vector3 localItemOffsetPos = localLinePos + (Vector3.forward * ad.GridSize * z * Mathf.Sign(deltaPos.z));
                        (GameObject g1, GameObject g2) g = bldr._drawingManager.CreateFloorItemInPosition(x, z, localItemOffsetPos.RoundDecimals(5));
                        sd.drawingFloorLinesItems[x].Add(g);
                    }
                }
            }

            lineNumber = sd.drawingFloorLinesItems.Count;

            if (xLength < lineNumber)
            {
                for (int x = lineNumber - 1; x >= xLength; x--)
                {
                    bldr._drawingManager.RemoveDestroyFloorLineAt(x);
                }
            }

            lineNumber = sd.drawingFloorLinesItems.Count;
            itemNumber = sd.drawingFloorLinesItems[0].Count;

            if (zLength < itemNumber)
            {
                for (int x = lineNumber - 1; x >= 0; x--)
                {
                    itemNumber = sd.drawingFloorLinesItems[x].Count;
                    for (int z = itemNumber - 1; z >= zLength; z--)
                    {
                        bldr._drawingManager.DestroyFloorItemFromLinesAt(x, z);
                    }
                }
            }
        }

        private void SetPointerStartPosition()
        {
            sd.mStartPosG = sd.mCurrentPosG;
            sd.mStartPosNG = sd.mCurrentPosNG;
        }
        private void ClearDrawingVariables()
        {
            sd.drawingGroupRoot = null;
            sd.drawingWallItems = null;
            sd.drawingFloorLinesItems = null;
            sd.drawingFloorLines = null;
            sd.drawingInArea = null;
        }
        private void RemoveDrawedObjects()
        {
            if (sd.drawingGroupRoot != null)
                GameObject.DestroyImmediate(sd.drawingGroupRoot);

            if (sd.drawingFloorLines != null && sd.drawingFloorLines.Count > 0)
                foreach (GameObject go in sd.drawingFloorLines)
                    GameObject.DestroyImmediate(go);
        }
    }
}










