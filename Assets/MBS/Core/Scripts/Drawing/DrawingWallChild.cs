#if UNITY_EDITOR
using UnityEngine;


namespace MBS
{
    public class DrawingWallChild : EditorBehaviour
    {
        [SerializeField] private DrawingWall _root;
        [SerializeField] public Mesh _originalMesh;
        [SerializeField] public bool _doModify = true;


        public DrawingWall RootDW
        {
            get
            {
                if (_root == null)
                    _root = this.GetComponentInParent<DrawingWall>();
                return _root;
            }
            set => _root = value;
        }

        public void SetupMesh(Mesh mesh = null)
        {
            if (mesh == null)
                mesh = _originalMesh;


            if (TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter.mesh = mesh;
            }

            MeshCollider meshCollider = GetComponent<MeshCollider>();

            if (meshCollider == null)
                meshCollider = this.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

    }
}
#endif