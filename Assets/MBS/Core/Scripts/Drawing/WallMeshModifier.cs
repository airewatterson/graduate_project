#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;

namespace MBS
{
    [Serializable]
    public class WallMeshModifier
    {
        public const string NULL_ORIGINAL_MESH = "Can not load original model from {0}";
        private const string CANT_FIND_ASSET_BY_GUID = "MBS. DrawingWall. Can't re-instantiate mesh, can't find asset with such guid";
        private const string NO_VERTECIES_IN_MESH = "MBS. DrawingWall. There are no vertecies in the mesh.";

        private static Color NO_AFFECT_COLOR = Color.black;


        public static Mesh ModifyMesh(Mesh originalMesh,
                                      MeshSideModification frontMod,
                                      MeshSideModification rearMod,
                                      Vector3 frontEndPoint_local,
                                      Vector3 rearEndPoint_local,
                                      Transform rootObject,
                                      Transform curObject,
                                      bool is45Degree)
        {
            bool hasColors = true;

            if (originalMesh == null)
            {
                Debug.LogError(NULL_ORIGINAL_MESH);
                return null;
            }

            if (originalMesh.vertices == null || originalMesh.vertices.Length == 0)
            {
                Debug.LogError(NO_VERTECIES_IN_MESH);
                return null;
            }

            if (originalMesh.colors == null || originalMesh.colors.Length == 0)
            {
                hasColors = false;
            }

            if (!is45Degree && frontMod.angle == 0 && rearMod.angle == 0)
            {
                return originalMesh;
            }

            Vector3[] vertices = originalMesh.vertices;
            Color[] colors = originalMesh.colors;

            if (is45Degree || hasColors)
            {
                if (rootObject != curObject)
                {
                    vertices = vertices.Select(i => curObject.transform.TransformPoint(i)).ToArray();
                    vertices = vertices.Select(i => rootObject.transform.InverseTransformPoint(i)).ToArray();
                }
            }

            if (is45Degree)
            {
                for (int v = 0; v < vertices.Length; v++)
                {
                    if (hasColors && colors[v] == NO_AFFECT_COLOR) continue;
                    vertices[v] = new Vector3(vertices[v].x / Mathf.Sin(45 * Mathf.Deg2Rad), vertices[v].y, vertices[v].z);
                }
            }

            if (hasColors)
            {
                Color frontColor = GetNearVertColor(frontEndPoint_local, vertices, colors);
                Color rearColor = GetNearVertColor(rearEndPoint_local, vertices, colors);
                float angle = 0;
                float sign = 0;

                for (int v = 0; v < vertices.Length; v++)
                {
                    if (colors[v] != Color.white && colors[v] != NO_AFFECT_COLOR)
                    {

                        if (colors[v] == frontColor)
                        {
                            angle = frontMod.angle;
                            if (vertices[v].z > 0)
                            {
                                sign = frontMod.positiveSide;
                            }
                            else if (vertices[v].z < 0)
                            {
                                sign = frontMod.negativeSide;
                            }
                            else if (vertices[v].z == 0)
                                sign = 0;
                        }
                        else if (colors[v] == rearColor)
                        {
                            angle = rearMod.angle;
                            if (vertices[v].z > 0)
                            {
                                sign = -rearMod.positiveSide;
                            }
                            else if (vertices[v].z < 0)
                            {
                                sign = -rearMod.negativeSide;
                            }
                            else if (vertices[v].z == 0)
                                sign = 0;
                        }

                        float offset = Mathf.Abs(vertices[v].z);

                        if (angle == 90) { }
                        else if (angle == 45)
                            offset = Mathf.Abs(offset + (offset / Mathf.Sin(Mathf.Deg2Rad * 45)));
                        else if (angle == 135)
                            offset = Mathf.Abs(offset - (offset / Mathf.Sin(Mathf.Deg2Rad * 45)));
                        else continue;

                        vertices[v].x += offset * sign;
                    }
                }
            }

            if (is45Degree || hasColors)
            {
                if (rootObject != curObject)
                {
                    vertices = vertices.Select(i => rootObject.transform.TransformPoint(i)).ToArray();
                    vertices = vertices.Select(i => curObject.transform.InverseTransformPoint(i)).ToArray();
                }
            }

            Mesh mesh = Mesh.Instantiate(originalMesh);
            mesh.name = "ModifiedMesh";
            mesh.SetVertices(vertices);
            mesh.RecalculateBounds();
            return mesh;
        }



        private static Color GetNearVertColor(Vector3 localPoint, Vector3[] vertecies, Color[] colors)
        {
            float minDistance = float.MaxValue;
            Color retval = Color.black;

            for (int i = 0; i < vertecies.Length; i++)
            {
                if (colors[i] == Color.white) continue;
                if (colors[i] == NO_AFFECT_COLOR) continue;

                float distnace = Vector3.Distance(localPoint, vertecies[i]);
                if (distnace < minDistance)
                {
                    minDistance = distnace;
                    retval = colors[i];
                }
            }
            return retval;
        }

        private static int GetNearestVertColorIndex(Vector3 localPoint, Vector3 nearCorner, Vector3[] vertecies, Color[] colors, Color excludeColor)
        {
            float minDistance = float.MaxValue;
            int retval = 0;
            for (int i = 0; i < vertecies.Length; i++)
            {
                if (colors[i] == excludeColor) continue;
                if (colors[i] == Color.white) continue;
                if (colors[i] == NO_AFFECT_COLOR) continue;

                if (nearCorner.z < 0 && vertecies[i].z <= nearCorner.z) continue;
                if (nearCorner.z > 0 && vertecies[i].z >= nearCorner.z) continue;

                float distnace = Vector3.Distance(localPoint, vertecies[i]);
                if (distnace < minDistance)
                {
                    minDistance = distnace;
                    retval = i;
                }
            }
            return retval;
        }


    }


}

#endif