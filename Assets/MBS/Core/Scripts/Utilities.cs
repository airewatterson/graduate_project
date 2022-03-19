#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MBS
{
    public class Utilities : MonoBehaviour
    {
        public static void AddTagIfNotExist(string tag)
        {
            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject so = new SerializedObject(asset[0]);
                SerializedProperty tags = so.FindProperty("tags");

                for (int i = 0; i < tags.arraySize; ++i)
                {
                    if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
                }

                tags.InsertArrayElementAtIndex(0);
                tags.GetArrayElementAtIndex(0).stringValue = tag;
                so.ApplyModifiedProperties();
                so.Update();
            }
        }

        public static Texture2D GetPreviewTexture(GameObject prefab)
        {
            if (prefab != null)
                return AssetPreview.GetAssetPreview(prefab);
            else return Texture2D.grayTexture;
        }

        public static Component CopyYourComponentTypeValues(Component source)
        {

            Component target = new Component();

            FieldInfo[] sourceFields = source.GetType().GetFields(BindingFlags.Public |
                                                                  BindingFlags.NonPublic |
                                                                  BindingFlags.Instance);

            int i = 0;

            for (i = 0; i < sourceFields.Length; i++)
            {
                var value = sourceFields[i].GetValue(source);
                sourceFields[i].SetValue(target, value);
            }

            return target;
        }
    }

    public static class ExtensionMethods
    {
        public static Vector3 RoundDecimals(this Vector3 vector3, int decimalPlaces = 2)
        {
            return new Vector3(
                (float)System.Math.Round(vector3.x, decimalPlaces),
                (float)System.Math.Round(vector3.y, decimalPlaces),
                (float)System.Math.Round(vector3.z, decimalPlaces));
        }

        public static int Remap(this int from, int fromMin, int fromMax, int toMin, int toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static float RoundDecimals(this float var, int decimalPlaces = 3)
        {
            return (float)System.Math.Round(var, decimalPlaces);
        }

        public static void SelectObject(this UnityEngine.Object obj)
        {
            Selection.objects = new[] { obj };
        }
        public static void AddToSelection(this UnityEngine.Object obj)
        {
            List<UnityEngine.Object> selected = Selection.objects.ToList();
            selected.Add(obj);
            Selection.objects = selected.ToArray();
        }

        public static void DoRecursive(this Transform root, Action<Transform> function)
        {
            foreach (Transform child in root)
            {
                function.Invoke(child);
                child.DoRecursive(function);
            }
        }
    }

    public class TransformData
    {
        public Vector3 LocalPosition = Vector3.zero;
        public Vector3 LocalEulerRotation = Vector3.zero;
        public Vector3 LocalScale = Vector3.one;

        public TransformData() { }

        public TransformData(Transform transform)
        {
            LocalPosition = transform.localPosition;
            LocalEulerRotation = transform.localEulerAngles;
            LocalScale = transform.localScale;
        }

        public void ApplyTo(Transform transform)
        {
            transform.localPosition = LocalPosition;
            transform.localEulerAngles = LocalEulerRotation;
            transform.localScale = LocalScale;
        }
    }

}
#endif
