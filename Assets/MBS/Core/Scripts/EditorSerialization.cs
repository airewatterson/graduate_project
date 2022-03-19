#if UNITY_EDITOR
using UnityEditor;

public static class EditorSerialization
{
    public static void RecordObjectUndo(this UnityEngine.Object obj, string text = null)
    {
        if (!string.IsNullOrEmpty(text))
            Undo.RecordObject(obj, text);
        else
            Undo.RecordObject(obj, obj.name);
    }

    public static void RecordCreatedUndo(this UnityEngine.Object obj, string text = null)
    {
        if (!string.IsNullOrEmpty(text))
            Undo.RegisterCreatedObjectUndo(obj, text);
        else
            Undo.RegisterCreatedObjectUndo(obj, obj.name);
    }

    public static void DestroyImmediateUndo(this UnityEngine.Object obj, UnityEngine.Object objectToDestroy = null)
    {
        if (objectToDestroy != null)
            Undo.DestroyObjectImmediate(objectToDestroy);
        else
            Undo.DestroyObjectImmediate(obj);
    }

    public static void FlushRecord(this UnityEngine.Object obj)
    {
        Undo.FlushUndoRecordObjects();
    }

    public static void MakeDirty(this UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
    }




}

#endif
