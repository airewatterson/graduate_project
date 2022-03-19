#if UNITY_EDITOR
using UnityEngine;

namespace MBS
{
    public class ZEnums : MonoBehaviour { }

    public enum DrawingCorner
    {
        TopLeft,
        BotLeft,
        TopRight,
        BotRight,
        All,
        None
    }

    public enum BuilderTools
    {
        Walls,
        Floors
    }

    public enum PrefabType
    {
        None,
        Basic,
        LodGroup
    }

    public enum FloorTileType
    {
        Square,
        Corner,
        None
    }

    public enum SceneViewMode
    {
        Idle,
        Drawing,
        PositionPicker,
        ObjectPicker
    }

    public enum BuilderParameters
    {
        GridSize,
        LevelHeight,
        None
    }


    public enum RaycastObjectParameter
    {
        ObjectHeight,
        XAxisSize
    }

}
#endif
