#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MBS
{
    [Serializable]
    public class AreaTriangle
    {
        private const string TRIANGLE_HASNO_POINTS = "MBS. Area Triangle has no points.";
        [SerializeField] public MBSBuilder builder;
        [SerializeField] public float area;
        [SerializeField] public Vector3[] points;
        [SerializeField] private List<AreaTriangle> trianglesInside;

        public AreaTriangle(Vector3 a, Vector3 b, Vector3 c, MBSBuilder builder)
        {
            this.builder = builder;
            points = new Vector3[3];
            points[0] = a;
            points[1] = b;
            points[2] = c;
            area = GetArea();
            trianglesInside = new List<AreaTriangle>();
        }

        public float GetArea()
        {
            float area = 0f;
            for (var i = 0; i < points.Length; i++)
            {
                var ni = (i + 1) % points.Length;
                area += points[i].x * points[ni].z - points[i].z * points[ni].x;
            }
            return Mathf.Abs(area / 2);
        }

        public float GetArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Mathf.Abs(((p1.x * p2.z) - (p1.z * p2.x)) +
                            ((p2.x * p3.z) - (p2.z * p3.x)) +
                            ((p3.x * p1.z) - (p3.z * p1.x)));
        }

        public (bool isInside, bool isEdge) IsPointInside(Vector3 point)
        {
            if (points == null || points.Length != 3)
            {
                Debug.Log(TRIANGLE_HASNO_POINTS);
                return (false, false);
            }

            float a1, a2, a3;

            a1 = GetArea(point, points[0], points[1]).RoundDecimals(4);
            a2 = GetArea(point, points[1], points[2]).RoundDecimals(4);
            a3 = GetArea(point, points[2], points[0]).RoundDecimals(4);

            float sum = ((a1 + a2 + a3) / 2).RoundDecimals();

            bool isInside = (sum == area);
            bool isEdge = (a1 == 0) || (a2 == 0) || (a3 == 0);

            return (isInside, isEdge);
        }

        public bool IsTriangleInside(AreaTriangle triangle)
        {
            if (trianglesInside == null || trianglesInside.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < trianglesInside.Count; i++)
            {
                if (triangle == trianglesInside[i]) return true;
            }
            return false;
        }

        public void AddIntersectedTriangle(AreaTriangle triagnle)
        {
            if (triagnle == null || triagnle.points.Length != 3) return;

            if (!trianglesInside.Contains(triagnle))
                trianglesInside.Add(triagnle);
        }


    }
}
#endif