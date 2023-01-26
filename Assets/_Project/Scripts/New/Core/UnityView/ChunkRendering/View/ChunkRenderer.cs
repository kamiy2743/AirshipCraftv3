using UnityEngine;
using Domain.Chunks;
using UnityView.ChunkRendering.Model.ChunkMesh;

namespace UnityView.ChunkRendering.View
{
    internal class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        private Mesh mesh;

        internal void SetRootCoordinate(ChunkGridCoordinate coordinate)
        {
        }

        internal void SetMesh(ChunkMesh chunkMesh)
        {
            if (chunkMesh.vertices.Length == 0 && mesh is not null)
            {
                Destroy(mesh);
                mesh = null;
                meshFilter.sharedMesh = null;
                transform.position = Vector3.zero;
                return;
            }

            if (mesh is null)
            {
                mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            mesh.SetVertices(chunkMesh.vertices);
            mesh.SetTriangles(chunkMesh.triangles, 0);
            mesh.SetUVs(0, chunkMesh.uvs);
            mesh.RecalculateNormals();

            transform.position = chunkMesh.rootPosition;
        }

        private void OnDestroy()
        {
            if (mesh is not null)
            {
                Destroy(mesh);
            }
        }
    }
}