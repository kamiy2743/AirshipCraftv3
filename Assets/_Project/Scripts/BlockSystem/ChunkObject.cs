using UnityEngine;
using System.Threading;

namespace BlockSystem
{
    internal class ChunkObject : MonoBehaviour, IBlockDataAccessor
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        private ChunkDataStore _chunkDataStore;

        internal void Init(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
            meshCollider.sharedMesh = null;
            meshCollider.enabled = false;
        }

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

            // meshCollider.sharedMesh = mesh;
            // meshCollider.enabled = true;
        }

        internal void ClearMesh()
        {
            meshFilter.mesh.Clear();
            meshCollider.sharedMesh = null;
            meshCollider.enabled = false;
        }

        public BlockData GetBlockData(Vector3 position, CancellationToken ct)
        {
            var bc = new BlockCoordinate(position);
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);
            var chunkData = _chunkDataStore.GetChunkData(cc, ct);

            return chunkData.GetBlockData(lc);
        }
    }
}
