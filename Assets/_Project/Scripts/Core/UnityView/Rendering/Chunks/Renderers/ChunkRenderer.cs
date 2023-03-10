using System;
using UnityEngine;
using UnityView.Rendering.Chunks;

namespace UnityView.Rendering.Chunks
{
    class ChunkRenderer : MonoBehaviour, IDisposable
    {
        [SerializeField] MeshFilter meshFilter;

        Mesh _mesh;

        internal void SetMesh(ChunkMesh chunkMesh)
        {
            if (chunkMesh.Vertices.Length == 0 && _mesh is not null)
            {
                Destroy(_mesh);
                _mesh = null;
                meshFilter.sharedMesh = null;
                transform.position = Vector3.zero;
                return;
            }

            if (_mesh is null)
            {
                _mesh = new Mesh();
                _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                meshFilter.sharedMesh = _mesh;
            }
            else
            {
                _mesh.Clear();
            }

            _mesh.SetVertices(chunkMesh.Vertices);
            _mesh.SetTriangles(chunkMesh.Triangles, 0);
            _mesh.SetUVs(0, chunkMesh.Uvs);
            _mesh.RecalculateNormals();

            transform.position = chunkMesh.ChunkGridCoordinate.ToPivotCoordinate();
        }

        public void Dispose()
        {
            if (_mesh is not null)
            {
                Destroy(_mesh);
            }
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}