using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MBS
{
    internal class EBuilder_Inspector
    {
        public const string ASSET_ITEMS_NOT_FOUND = "Asset Items not found";

        public MBSBuilder bldr;
        public AssetsData ad;
        public SceneData sd;

        float viewWidth;
        float assetsGridOffset = 155;
        float spaceInPixels;


        internal void InspectorBootstrap(MBSBuilder builder)
        {
            this.bldr = builder;
            this.ad = builder._assetsData;
            this.sd = builder._sceneData;

            viewWidth = EditorGUIUtility.currentViewWidth - 25;
            spaceInPixels = viewWidth / 6;


            ShowToolbar();

            switch (ad.Current_Tool)
            {
                case BuilderTools.Walls:
                    ShowParameters();
                    ShowAssetPacksList();
                    ShowAssetsSelectionGrid();
                    break;
                case BuilderTools.Floors:
                    ShowParameters();
                    ShowAssetPacksList();
                    ShowAssetsSelectionGrid();
                    break;
                default:
                    break;
            }
        }

        private void ShowToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();
            {
                BuilderTools tool = (BuilderTools)GUILayout.Toolbar(ad.Current_Tool_Int,
                                                                    ad.ToolbarContent,
                                                                    Styles.Toolbar(ad.ToolbarContent.Length, viewWidth),
                                                                    GUILayout.MaxHeight(28));
                if (EditorGUI.EndChangeCheck())
                {
                    ad.Current_Tool = tool;
                    bldr.ChangeGizmo(tool);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        private void ShowParameters()
        {
            EditorGUILayout.LabelField("Building Settings ", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(6, false);
            EditorGUILayout.BeginVertical();

            GUIStyle toolbarStyle = new GUIStyle();
            toolbarStyle.margin = new RectOffset(0, 0, -10, 0);



            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginHorizontal();
                float gridSize = EditorGUILayout.FloatField("Grid Size ", ad.GridSize);
                if (EditorGUI.EndChangeCheck())
                {
                    bldr.GridMan_SetParams(gridSize: gridSize);
                }

            }
            EditorGUI.BeginChangeCheck();
            {
                int gridSizeButtons = GUILayout.Toolbar(sd.gridSizeButtons, ad.GridSizePickerContent, GUILayout.MaxWidth(64), GUILayout.MaxHeight(20));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    if (gridSizeButtons == 0)
                    {
                        if (sd.gridSizeButtons == 0)
                        {
                            sd.SetIdleMode();
                            sd.gridSizeButtons = -1;
                        }
                        else
                        {
                            sd.heightButtons = -1;
                            sd.gridSizeButtons = 0;
                        }
                    }
                    else if (gridSizeButtons == 1)
                    {
                        if (sd.gridSizeButtons == 1)
                        {
                            sd.gridSizeButtons = -1;
                            sd.SetIdleMode();
                        }
                        else
                        {
                            sd.heightButtons = -1;

                            sd.SetObjPickerMode(BuilderParameters.GridSize);
                            sd.gridSizeButtons = 1;
                        }
                    }
                }
            }
            if (sd.gridSizeButtons == 0)
            {
                bldr.GridMan_SetParams(gridSize: ad.Current_Asset_Size.x);
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginHorizontal();
                float levelHeight = EditorGUILayout.FloatField("Level Height ", ad.LevelHeight);
                if (EditorGUI.EndChangeCheck())
                {
                    bldr.GridMan_SetParams(levelHeight: levelHeight);
                }
            }

            EditorGUI.BeginChangeCheck();
            {
                int levelHeightButtons = GUILayout.Toolbar(sd.heightButtons, ad.HeightPickerContent, GUILayout.MaxWidth(64), GUILayout.MaxHeight(20));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    if (levelHeightButtons == 0)
                    {
                        if (sd.heightButtons == 0)
                        {
                            sd.SetIdleMode();
                            sd.heightButtons = -1;
                        }
                        else
                        {
                            if (sd.gridSizeButtons != 0)
                                sd.gridSizeButtons = -1;

                            sd.SetPosPickerMode();
                            sd.heightButtons = 0;
                        }
                    }
                    else if (levelHeightButtons == 1)
                    {
                        if (sd.heightButtons == 1)
                        {
                            sd.SetIdleMode();
                            sd.heightButtons = -1;
                        }
                        else
                        {
                            if (sd.gridSizeButtons != 0)
                                sd.gridSizeButtons = -1;

                            sd.SetObjPickerMode(BuilderParameters.LevelHeight);
                            sd.heightButtons = 1;
                        }
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginHorizontal();
                int levelNumber = EditorGUILayout.IntField("Level Number ", ad.LevelNumber);
                int levelNumberButtons = GUILayout.Toolbar(-1, new[] { "-", "+" }, GUILayout.MaxWidth(64), GUILayout.MaxHeight(20));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    if (levelNumber == ad.LevelNumber)
                    {
                        if (levelNumberButtons == 1) levelNumber++;
                        else if (levelNumberButtons == 0) levelNumber--;
                    }
                    bldr.GridMan_SetParams(levelNumber: levelNumber);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);
        }
        private void ShowAssetPacksList()
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle("label");
            style.fixedWidth = 128;
            EditorGUILayout.LabelField("Asset Packs ", GUILayout.MaxWidth(134));
            EditorGUI.BeginChangeCheck();
            {
                int packIndex = EditorGUILayout.Popup(ad.Current_AssetPack_Index, MBSAssetsManager.Singleton.AssetPacksNames);
                if (EditorGUI.EndChangeCheck())
                {
                    ad.Try_SetupCurrentAssetPack(ad.Current_Tool_Int, packIndex);
                    ad.Try_SetupCurrentAsset(ad.Current_Tool_Int, 0);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ShowAssetsSelectionGrid()
        {
            AssetsData astd = bldr._assetsData;

            GUIStyle horStyle = new GUIStyle();
            horStyle.stretchHeight = true;
            int co = ad.Current_AssetPack_AssetsLength();
            int columns = (int)(viewWidth - assetsGridOffset) / 54;
            int rows = (int)Mathf.Ceil((co + columns - 1) / columns);
            int clampedHeight = Mathf.Clamp(60 * rows, 140, 250);
            EditorGUILayout.BeginHorizontal(horStyle, GUILayout.Height(clampedHeight));
            {
                GUIStyle st = new GUIStyle();
                st.margin = new RectOffset(0, 0, 10, 0);
                Rect itemPreviewRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(128), GUILayout.Height(128));
                itemPreviewRect.position = itemPreviewRect.position + Vector2.up * 5;
                GameObject prefab = astd.Current_Asset_FirstPrefab();
                EditorGUI.DrawPreviewTexture(itemPreviewRect, MBSEditorTools.GetAssetPreviewOrGray(prefab));

                astd.ScrollPos = EditorGUILayout.BeginScrollView(astd.ScrollPos, horStyle, GUILayout.MaxHeight(230));
                EditorGUI.BeginChangeCheck();
                {
                    int itemIndex = AssetItemSelectionGrid(astd.Current_Asset_Index, astd.Current_AssetPack_Assets(), 54, viewWidth);

                    if (EditorGUI.EndChangeCheck())
                    {
                        astd.Try_SetupCurrentAsset(astd.Current_Tool_Int, itemIndex);
                        bldr.ChangeGizmo(astd.Current_Tool);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }
        private int AssetItemSelectionGrid(int selected, MBSAsset[] assetItems, int buttonSize, float viewWidth)
        {
            GUILayout.BeginVertical("box", GUILayout.MinHeight(128));
            GUIStyle style = new GUIStyle("button");
            style.fixedWidth = buttonSize;
            style.fixedHeight = buttonSize;

            int retval = 0;

            if (assetItems.Length != 0)
            {
                GUIContent[] thumbnails = new GUIContent[assetItems.Length];

                for (int i = 0; i < thumbnails.Length; i++)
                {
                    GameObject gameObject = assetItems.ElementAtOrDefault(i).Prefabs.FirstOrDefault();
                    string text = assetItems[i].name;
                    if (string.IsNullOrEmpty(text)) text = gameObject.name;
                    if (string.IsNullOrEmpty(text)) text = "Unnamed asset";
                    thumbnails[i] = new GUIContent(AssetPreview.GetAssetPreview(gameObject), text);
                }

                int columns = (int)(viewWidth - 155) / buttonSize;
                int rows = (int)Mathf.Ceil((assetItems.Length + columns - 1) / columns);
                Rect rect = GUILayoutUtility.GetAspectRect((float)columns / (float)rows);
                retval = GUI.SelectionGrid(rect, System.Math.Min(selected, assetItems.Length - 1), thumbnails, (int)columns, style);
            }
            else
            {
                EditorGUILayout.HelpBox(ASSET_ITEMS_NOT_FOUND, MessageType.Error);
            }

            GUILayout.EndVertical();
            return retval;
        }

        private static class Styles
        {
            public static GUIStyle Toolbar(int iconsNumber, float viewWidth)
            {
                GUIStyle retval = new GUIStyle("button");
                retval.fixedWidth = 60;
                retval.fixedHeight = 26;
                return retval;
            }

            public static GUIStyle DrawingLevelButton(float singleButtonWidth)
            {
                GUIStyle retval = new GUIStyle("button");
                GUIStyle asd = EditorStyles.toolbarButton;
                retval.fixedHeight = EditorGUIUtility.singleLineHeight + 1;
                retval.fixedWidth = singleButtonWidth;
                retval.clipping = TextClipping.Clip;
                retval.imagePosition = ImagePosition.ImageOnly;
                retval.margin = new RectOffset();
                retval.padding = new RectOffset();
                retval.overflow = new RectOffset();
                retval.stretchHeight = false;
                retval.stretchWidth = false;
                return retval;
            }

            public static GUIStyle ToolbarsItemAlignPivot(float singleButtonWidth)
            {
                GUIStyle retval = new GUIStyle("button");
                retval.fixedHeight = EditorGUIUtility.singleLineHeight + 1;
                retval.fixedWidth = singleButtonWidth;
                retval.margin.left = 6;
                return retval;
            }
        }
    }
}
