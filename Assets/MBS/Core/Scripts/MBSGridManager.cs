#if UNITY_EDITOR
using UnityEngine;

namespace MBS
{
    public class MBSGridManager
    {
        private const string BUILDER_CONFIG_NULL = "MBS. Config object is null. GridManager initialization can't be done.";
        private const string GRID_MESH_NULL = "MBS. Grid mesh is null. GridManager initialization can't be done.";
        private const string GRID_MATERIAL_NULL = "MBS. Grid material is null. GridManager initialization can't be done.";

        MBSBuilder builder;

        private float gridSize;
        private float meshScale;
        private Vector3 position;

        private float height;

        private Mesh gridMesh;
        private Material gridMaterial;


        public float GridSize
        {
            get => gridSize;
            set
            {
                if (value != gridSize)
                {
                    gridSize = Mathf.Max(1, value);
                    ChangeGridSize();
                }
            }
        }

        public float MeshSize
        {
            get => meshScale;
            set
            {
                if (value != meshScale)
                {
                    meshScale = value;
                    ChangeMeshScale();
                }
            }
        }

        public Vector3 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = new Vector3(value.x, height, value.z);
                    ChangeGridPosition();
                }
            }
        }

        public float Height
        {
            get => position.y;
            set
            {
                height = value;
                position.y = value;
                ChangeGridPosition();
            }
        }

        public MBSGridManager(MBSBuilder builder)
        {
            this.builder = builder;
            this.position = Vector3.zero;
            this.gridSize = 1;
            this.height = 0;
        }

        public void Initialize()
        {
            if (MBSConfig.Singleton == null)
            {
                Debug.LogError(BUILDER_CONFIG_NULL);
                return;
            }
            if (MBSConfig.Singleton.gridMesh == null)
            {
                Debug.LogError(GRID_MESH_NULL);
                return;
            }
            if (MBSConfig.Singleton.gridMaterial == null)
            {
                Debug.LogError(GRID_MATERIAL_NULL);
                return;
            }

            this.gridMesh = MBSConfig.Singleton.gridMesh;
            this.gridMaterial = MBSConfig.Singleton.gridMaterial;
            ChangeGridPosition();
            ChangeGridSize();

            if (!builder.gameObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                meshFilter = builder.gameObject.AddComponent<MeshFilter>();

            if (!builder.gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
                meshRenderer = builder.gameObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = gridMesh;
            meshRenderer.material = gridMaterial;
        }

        private void ChangeGridSize()
        {
            if (gridMaterial == null) Initialize();
            if (gridMaterial != null)
                gridMaterial.SetFloat("_GridScale", gridSize);
        }

        private void ChangeMeshScale()
        {
            if (gridMaterial == null) Initialize();
            if (gridMaterial != null)
                gridMaterial.SetFloat("_MeshScale", meshScale);
        }

        private void ChangeGridPosition()
        {
            if (gridMaterial == null) Initialize();
            if (gridMaterial != null)
            {
                gridMaterial.SetVector("_GridPosition", position);
            }
        }
    }
}

#endif


