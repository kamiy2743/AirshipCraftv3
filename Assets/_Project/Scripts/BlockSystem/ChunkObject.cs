using UnityEngine;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        internal void SetMesh(ChunkMeshData meshData)
        {
            var mesh = meshFilter.mesh;
            mesh.Clear();

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.RecalculateNormals();
        }
    }
}
