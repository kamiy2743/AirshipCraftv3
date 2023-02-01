using System;
using UnityEngine;
using UnityView.Rendering.Chunks;

namespace UnityView.Rendering.Chunks
{
    internal class ChunkRenderer : MonoBehaviour, IDisposable
    {
        [SerializeField] private MeshFilter meshFilter;

        private UnityEngine.Mesh mesh;

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
                mesh = new UnityEngine.Mesh();
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

            transform.position = chunkMesh.chunkGridCoordinate.ToPivotCoordinate();
        }

        public void Dispose()
        {
            if (mesh is not null)
            {
                Destroy(mesh);
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}