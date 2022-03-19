#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace MBS
{
    public class SceneData
    {

        public bool pointerEnabled;
        public bool drawingAt45Deg;

        public Event evnt;

        public Area drawingInArea;
        public GameObject drawingGroupRoot;

        public List<GameObject> drawingWallItems;

        public int currentLineIndex;
        public List<GameObject> drawingFloorLines;
        public List<List<(GameObject g1, GameObject g2)>> drawingFloorLinesItems;
        public List<List<Vector3>> tileLocalPositions;
        public List<List<bool>> drawingFloorTileChecker;


        public Vector3 mStartPosG;
        public Vector3 mStartPosNG;

        public Vector3 mCurrentPosG;
        public Vector3 mCurrentPosNG;

        public Vector3 mPrevPosG;


        public Vector3 pickerPos;
        public Vector3 ObjPicker_ObjCenterLocal;
        public Vector3 ObjPicker_ObjExtents;
        public GameObject ObjPicker_Object;
        public bool isThereObject_ObjPicker;


        public Vector3 drawingItemOffset;

        public bool doSnapToEndPoints = true;

        public bool ctrlPressed;
        public bool altPressed;
        public bool shiftPressed;


        public SceneViewMode sceneMode;
        public BuilderParameters builderParameter;

        public int heightButtons = -1;
        public int gridSizeButtons = -1;
        public int gridSizeLinkButton = -1;

        public bool gridSizeLinked;

        public bool isIdleMode { get => sceneMode == SceneViewMode.Idle; }
        public bool isDrawingMode { get => sceneMode == SceneViewMode.Drawing; }
        public bool isPosPickerMode { get => sceneMode == SceneViewMode.PositionPicker; }
        public bool isObjPickerMode { get => sceneMode == SceneViewMode.ObjectPicker; }

        public void SetIdleMode()
        {
            heightButtons = -1;
            builderParameter = BuilderParameters.None;
            ObjPicker_Object = null;
            ObjPicker_ObjCenterLocal = Vector3.zero;
            sceneMode = SceneViewMode.Idle;
        }
        public void SetDrawingMode() => sceneMode = SceneViewMode.Drawing;
        public void SetPosPickerMode() => sceneMode = SceneViewMode.PositionPicker;
        public void SetObjPickerMode(BuilderParameters parameter)
        {
            sceneMode = SceneViewMode.ObjectPicker;
            builderParameter = parameter;
        }

    }
}
#endif