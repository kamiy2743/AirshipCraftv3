using UnityEngine;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        internal void SetMesh(Mesh mesh)
        {
            meshFilter.mesh = mesh;
        }
    }
}
