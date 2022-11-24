using UnityEngine;
using System.Threading;

namespace BlockSystem
{
    /// <summary> チャンクのGameObject </summary>
    internal class ChunkObject : MonoBehaviour, IBlockDataAccessor
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        private ChunkDataStore _chunkDataStore;
        private Mesh mesh;

        internal void Init(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        /// <summary> メインスレッドのみ </summary>
        internal void SetMesh(ChunkMeshData meshData)
        {
            if (meshData is null)
            {
                ClearMesh();
                return;
            }

            if (mesh is null)
            {
                mesh = new Mesh();
                meshFilter.sharedMesh = mesh;
                meshRenderer.enabled = true;
            }

            mesh.Clear();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();

            meshData.Release();
        }

        internal void ClearMesh()
        {
            if (mesh is not null)
            {
                Destroy(mesh);
                mesh = null;
                meshRenderer.enabled = false;
                meshCollider.enabled = false;
                meshCollider.sharedMesh = null;
            }
        }

        internal void SetColliderEnabled(bool enabled)
        {
            if (meshCollider.enabled == enabled) return;

            meshCollider.enabled = enabled;
            meshCollider.sharedMesh = (enabled ? mesh : null);
        }

        public BlockData GetBlockData(Vector3 position, CancellationToken ct)
        {
            var bc = new BlockCoordinate(position);
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);
            var chunkData = _chunkDataStore.GetChunkData(cc, ct);

            return chunkData.GetBlockData(lc);
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
