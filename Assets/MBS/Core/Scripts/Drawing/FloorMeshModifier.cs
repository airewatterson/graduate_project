#if UNITY_EDITOR
using UnityEngine;

namespace MBS
{

    public class FloorMeshModifier
    {
        public static void ModifyFloorCorner(GameObject floorPrefab, DrawingCorner corner, PrefabType prefabType)
        {
            switch (prefabType)
            {
                case PrefabType.Basic:
                    ModifyObject(floorPrefab, corner);
                    floorPrefab.transform.DoRecursive((Transform t) =>
                    {
                        ModifyObject(t.gameObject, corner);
                    });
                    break;
                case PrefabType.LodGroup:
                    floorPrefab.transform.DoRecursive((Transform t) =>
                    {
                        ModifyObject(t.gameObject, corner);
                    });
                    break;
            }
        }

        private static void ModifyObject(GameObject gameObject, DrawingCorner corner)
        {
            if (gameObject.TryGetComponent(out MeshFilter filter))
            {
                Mesh mesh = Mesh.Instantiate(filter.sharedMesh);

                Vector3 scaleVector = Vector3.one;
                Vector3[] verticies = mesh.vertices;
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;

                if (corner == DrawingCorner.TopRight)
                {
                    scaleVector = Vector3.one;
                }
                else if (corner == DrawingCorner.BotRight)
                {
                    scaleVector = new Vector3(1, 1, -1);
                    triangles = ReorderTriangles(triangles);
                }
                else if (corner == DrawingCorner.BotLeft)
                {
                    scaleVector = new Vector3(-1, 1, -1);
                }
                else if (corner == DrawingCorner.TopLeft)
                {
                    scaleVector = new Vector3(-1, 1, 1);
                    triangles = ReorderTriangles(triangles);
                }

                verticies = ModifyVertecies(verticies, scaleVector);
                normals = ModifyVertecies(normals, scaleVector);

                mesh = Mesh.Instantiate(filter.sharedMesh);
                mesh.name = "ModifiedFloorMesh";

                mesh.SetNormals(normals);
                mesh.SetVertices(verticies);
                mesh.SetTriangles(triangles, 0);

                gameObject.GetComponent<MeshFilter>().mesh = mesh;
            }
        }

        public static Vector3[] ModifyVertecies(Vector3[] vertecies, Vector3 scaleVector)
        {
            for (int i = 0; i < vertecies.Length; i++)
            {
                vertecies[i] = new Vector3(vertecies[i].x * scaleVector.x,
                                           vertecies[i].y * scaleVector.y,
                                           vertecies[i].z * scaleVector.z);
            }
            return vertecies;
        }

        public static Vector3[] ModifyNormals(Vector3[] normals, Vector3 scaleVector)
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] *= scaleVector.x * scaleVector.z;
            }
            return normals;
        }

        public static int[] ReorderTriangles(int[] trinagles)
        {
            for (int i = 0; i < trinagles.Length; i += 3)
            {
                int a = trinagles[i];
                int b = trinagles[i + 1];
                int c = trinagles[i + 2];

                trinagles[i] = a;
                trinagles[i + 1] = c;
                trinagles[i + 2] = b;
            }
            return trinagles;
        }
    }
}
#endif
