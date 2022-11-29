using UnityEngine;
using System.Threading;
using DataObject.Block;
using DataObject.Chunk;
using Util;

namespace DataObject.Chunk
{
    /// <summary> チャンクのGameObject </summary>
    public class ChunkObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        private Mesh mesh;

        /// <summary> メインスレッドのみ </summary>
        public void SetMesh(NativeMeshData meshData)
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
            meshCollider.sharedMesh = mesh;
        }

        public void ClearMesh()
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

        public void SetColliderEnabled(bool enabled)
        {
            if (meshCollider.enabled == enabled) return;

            meshCollider.enabled = enabled;
            meshCollider.sharedMesh = (enabled ? mesh : null);
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
