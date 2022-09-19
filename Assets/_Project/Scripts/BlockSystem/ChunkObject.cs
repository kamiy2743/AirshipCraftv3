using UnityEngine;

namespace BlockSystem
{
    internal class ChunkObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        internal void SetMesh(ChunkMeshData meshData)
        {
            var mesh = meshFilter.mesh;
            mesh.Clear();

            if (meshData.IsEmpty)
            {
                meshCollider.sharedMesh = null;
                meshCollider.enabled = false;
                return;
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();

            meshCollider.sharedMesh = mesh;
            meshCollider.enabled = true;
        }
    }
}
