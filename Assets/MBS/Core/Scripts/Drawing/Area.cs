#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MBS
{
    [Serializable]
    public class Area
    {
        private const string NO_PATH_POINTS = "MBS. Area has no path points. Can't check if point inside area.";
        private const string NO_TRIANGLES = "MBS. Area has no triangles. Can't check if point is inside area.";
        private const string NO_EXTREME_POINTS_INDEXES = "MBS. Area has no extreme points indexes. Can't return extreme points.";



        public AreaManager manager;
        public List<int> extPointsIndexes;
        public List<Vector3> pathPoints;
        private List<DrawingWall> items;
        public Vector2 areaSize;
        public float area;
        public float height;

        public Area hiddenParent;
        public List<Area> childAreas;
        public List<Area> innerAreas;
        public List<AreaTriangle> triangles;

        public List<DrawingWall> Items
        {
            get
            {
                if (items == null)
                    items = new List<DrawingWall>();
                else
                    items = items.Where(i => i != null).ToList();

                return items;
            }

            set
            {
                if (value == null)
                    return;
                else items = value;
            }
        }


        public Area(List<DrawingWall> items, AreaManager areaManager)
        {
            this.manager = areaManager;
            this.Items = items;
            this.childAreas = new List<Area>();
            this.pathPoints = GetItemEndPoints();
            this.extPointsIndexes = GetExtremePointsIndexes();
            this.triangles = BuildTriangles();
            this.areaSize = GetAreaSize();
            this.area = CalcArea(GetExtremePoints());
            this.height = areaManager.builder.LPos(items[0].transform.position).y;
            this.innerAreas = new List<Area>();
            this.AttachToItems();
            this.FaceAreaInside();
        }


        public List<Vector3> GetItemEndPoints()
        {
            List<Vector3> retval = new List<Vector3>();
            for (int i = 0; i < Items.Count; i++)
            {
                retval.Add(Items[i].RearEndPoint);
                retval.Add(Items[i].FrontEndPoint);
            }
            return retval.Distinct().ToList();
        }

        public List<int> GetExtremePointsIndexes()
        {
            if (pathPoints == null || pathPoints.Count < 3)
            {
                Debug.LogError(NO_PATH_POINTS);
                return null;
            }

            int prev, next;
            List<int> retval = new List<int>();

            for (int i = 0; i < pathPoints.Count; i++)
            {
                prev = ((i - 1) < 0) ? pathPoints.Count - 1 : (i - 1) % pathPoints.Count;
                next = (i + 1) % pathPoints.Count;

                Vector3 a = pathPoints[i] - pathPoints[prev];
                Vector3 b = pathPoints[i] - pathPoints[next];

                float angle = Vector3.Angle(b, a);

                if (angle != 0 && (angle % 180) != 0)
                {
                    retval.Add(i);
                }
            }

            return retval;
        }

        public static float CalcArea(Vector3[] extPoints)
        {
            if (extPoints == null || extPoints.Length == 0) return -1;

            float area = 0f;
            int numPoints = extPoints.Length;
            for (var i = 0; i < numPoints; i++)
            {
                var nexti = (i + 1) % numPoints;
                area += extPoints[i].x * extPoints[nexti].z - extPoints[i].z * extPoints[nexti].x;
            }
            return Mathf.Abs(area / 2);
        }

        public static Vector3[] GetExtremePoints(List<Vector3> pathPoints)
        {
            if (pathPoints == null || pathPoints.Count <= 2)
            {
                Debug.LogError(NO_PATH_POINTS);
                return null;
            }

            List<Vector3> retval = new List<Vector3>();
            int prev, next;
            for (int i = 0; i < pathPoints.Count; i++)
            {
                prev = ((i - 1) < 0) ? (pathPoints.Count - 1) : (i - 1);
                next = (i + 1) % pathPoints.Count;
                Vector3 a = pathPoints[i] - pathPoints[prev];
                Vector3 b = pathPoints[i] - pathPoints[next];
                float angle = Vector3.Angle(b, a);
                if (angle != 0 && (angle % 180) != 0)
                {
                    retval.Add(pathPoints[i]);
                }
            }
            return retval.ToArray();
        }

        public Vector3[] GetExtremePoints()
        {
            if (extPointsIndexes == null || extPointsIndexes.Count <= 2)
            {
                Debug.LogError(NO_EXTREME_POINTS_INDEXES);
                return null;
            }

            if (pathPoints == null || pathPoints.Count <= 2)
            {
                Debug.LogError(NO_PATH_POINTS);
                return null;
            }

            List<Vector3> retval = new List<Vector3>();
            for (int i = 0; i < extPointsIndexes.Count; i++)
            {
                if (i < pathPoints.Count)
                {
                    Vector3 selected = pathPoints[extPointsIndexes[i]];
                    if (selected != null)
                    {
                        retval.Add(selected);
                    }
                }
            }
            return retval.ToArray();
        }

        public void AttachToItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null) continue;
                Items[i].AttachArea(this);
            }
        }

        public void DetachFromItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null) continue;
                Items[i].DetachArea(this);
            }
        }

        public void OnDestroyItem(DrawingWall item)
        {
            if (item == null) return;
            Items = Items.Where((i) => i != item).ToList();
            item.Builder._areaManager.RemoveArea(this);
            DetachFromItems();
            UpdateParent();
        }

        public void RefreshItemsLoops()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null) continue;
            }
        }

        public void SelectAreaItems()
        {
            Selection.objects = Items.Select(i => i.gameObject).ToArray();
        }

        public bool CheckContinuity()
        {
            bool continuity = true;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null) continuity = false;
                else if (Items[i].FrontEndPoint == null || Items[i].FrontConnections.Count == 0) continuity = false;
                else if (Items[i].RearEndPoint == null || Items[i].RearConnections.Count == 0) continuity = false;
            }
            return continuity;
        }

        public void UpdateParent()
        {
            if (hiddenParent == null) return;
        }

        public void DestroyArea()
        {
            DetachFromItems();
            manager.RemoveArea(this);
        }

        private List<AreaTriangle> BuildTriangles()
        {
            if (extPointsIndexes == null || extPointsIndexes.Count < 3)
            {
                Debug.LogError(NO_EXTREME_POINTS_INDEXES);
                return null;
            }

            List<AreaTriangle> triangles = new List<AreaTriangle>();
            int n1, n2;

            for (int n = 0; n < extPointsIndexes.Count - 2; n++)
            {
                n1 = (n + 1) % extPointsIndexes.Count;
                n2 = (n + 2) % extPointsIndexes.Count;
                AreaTriangle t = new AreaTriangle(
                                pathPoints[extPointsIndexes[0]],
                                pathPoints[extPointsIndexes[n1]],
                                pathPoints[extPointsIndexes[n2]],
                                manager.builder);
                if (t.area > 0)
                    triangles.Add(t);
            }

            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles.Count; j++)
                {
                    if (i == j) continue;

                    bool isIntersect = false;

                    Vector3[] intersectedPoints = triangles[i].points.Intersect(triangles[j].points).ToArray();
                    if (intersectedPoints.Length > 0 && intersectedPoints.Length < 3)
                    {
                        Vector3[] remainedPointsI = triangles[i].points.Where(e => !intersectedPoints.Any(d => d == e)).ToArray();
                        Vector3[] remainedPointsJ = triangles[j].points.Where(e => !intersectedPoints.Any(d => d == e)).ToArray();

                        for (int r = 0; r < remainedPointsJ.Length; r++)
                        {
                            var isInsinde = triangles[i].IsPointInside(remainedPointsJ[r]);
                            isIntersect |= (isInsinde.isInside && isInsinde.isEdge == false);
                        }

                        if (isIntersect)
                        {
                            triangles[i].AddIntersectedTriangle(triangles[j]);
                            triangles[j].AddIntersectedTriangle(triangles[i]);
                        }
                        else if (intersectedPoints.Length == 2 && remainedPointsI.Length == 1 && remainedPointsJ.Length == 1)
                        {
                            Vector3 a = intersectedPoints[0];
                            Vector3 b = intersectedPoints[1];

                            Vector3 c = remainedPointsI[0];
                            Vector3 d = remainedPointsJ[0];
                            Vector3 AB = b - a;
                            Vector3 AC = c - a;
                            Vector3 AD = d - a;

                            float angle_с = (float)System.Math.Round(Vector3.SignedAngle(AB, AC, Vector3.up), 3);
                            float angle_В = (float)System.Math.Round(Vector3.SignedAngle(AB, AD, Vector3.up), 3);

                            float side_c = (float)System.Math.Round(AB.magnitude * AC.magnitude * Mathf.Sin(angle_с * Mathf.Deg2Rad), 3);
                            float side_d = (float)System.Math.Round(AB.magnitude * AD.magnitude * Mathf.Sin(angle_В * Mathf.Deg2Rad), 3);

                            if ((side_c >= 0 && side_d >= 0)
                                || (side_c <= 0 && side_d <= 0))
                            {
                                triangles[i].AddIntersectedTriangle(triangles[j]);
                                triangles[j].AddIntersectedTriangle(triangles[i]);
                            }
                        }
                    }
                }
            }

            return triangles;
        }

        private bool IsPointOnPathPoints(Vector3 point)
        {
            if (pathPoints == null || pathPoints.Count == 0)
            {
                Debug.LogError(NO_PATH_POINTS);
                return false;
            }

            for (int i = 0; i < pathPoints.Count; i++)
            {
                if (pathPoints[i] == point) return true;
            }

            return false;
        }

        public bool IsPointInsideArea(Vector3 pointer)
        {
            if (triangles == null || triangles.Count == 0)
            {
                Debug.LogError(NO_TRIANGLES);
                return false;
            }

            if (pointer.y != this.height)
            {
                return false;
            }

            if (IsPointOnPathPoints(pointer))
            {
                return true;
            }

            int edgeNumber = 0;
            int insideNumber = 0;
            List<AreaTriangle> insideTriangles = new List<AreaTriangle>();
            List<AreaTriangle> trisOnEdge = new List<AreaTriangle>();

            for (int i = 0; i < triangles.Count; i++)
            {
                var checker = triangles[i].IsPointInside(pointer);

                if (checker.isInside)
                {
                    insideNumber++;

                    if (checker.isEdge)
                    {
                        edgeNumber++;
                        trisOnEdge.Add(triangles[i]);
                    }
                }
            }

            if (insideNumber > 1 && edgeNumber > 1)
            {
                int trisIntersection = 0;

                trisOnEdge.Distinct();

                for (int j = 0; j < trisOnEdge.Count; j++)
                {
                    for (int k = 0; k < trisOnEdge.Count; k++)
                    {
                        if (j == k) continue;

                        if (trisOnEdge[j].IsTriangleInside(trisOnEdge[k]))
                            trisIntersection++;
                    }
                }

                if (trisIntersection == 0)
                    insideNumber--;
            }

            bool isInside = (insideNumber % 2) != 0;

            if (!isInside)
                if (edgeNumber == 1) isInside = true;
            return isInside;
        }

        public Vector2 GetAreaSize()
        {
            Vector2 retval;
            float xMax = float.MinValue;
            float yMax = float.MinValue;
            if (extPointsIndexes != null && extPointsIndexes.Count > 2)
            {
                for (int i = 0; i < extPointsIndexes.Count; i++)
                {
                    for (int j = 0; j < extPointsIndexes.Count; j++)
                    {
                        if (i == j) continue;
                        Vector3 diff = pathPoints[extPointsIndexes[j]] - pathPoints[extPointsIndexes[i]];
                        float xDiff = Mathf.Abs(diff.x);
                        float yDiff = Mathf.Abs(diff.z);
                        if (xDiff > xMax) xMax = xDiff;
                        if (yDiff > yMax) yMax = yDiff;
                    }
                }
                retval = new Vector2(xMax, yMax);
                return retval;
            }
            return default(Vector2);
        }

        public void FaceAreaInside()
        {
            if (Items == null || Items.Count == 0)
            {
                return;
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].FaceWallInsideArea(this);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                manager.builder._drawingManager.AddDrawingWall(Items[i], true, false);
            }
        }

    }
}
#endif
