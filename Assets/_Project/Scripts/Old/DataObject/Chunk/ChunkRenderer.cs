using UnityEngine;
using Util;

namespace DataObject.Chunk
{
    public class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        private Mesh mesh;

        /// <summary> 
        /// メインスレッドのみ 
        /// </summary>
        public void SetMesh(NativeMeshData meshData)
        {
            if (meshData is null || meshData.Vertices.Length == 0)
            {
                ClearMesh();
                return;
            }

            if (mesh is null)
            {
                mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                meshFilter.sharedMesh = mesh;
                meshRenderer.enabled = true;
            }
            else
            {
                mesh.Clear();
            }

            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();
        }

        public void ClearMesh()
        {
            if (mesh is null) return;

            Destroy(mesh);
            mesh = null;
            meshRenderer.enabled = false;
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
