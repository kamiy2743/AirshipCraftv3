using UnityEngine;
using Util;

namespace BlockBehaviour
{
    public class MUCoreObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;

        private Mesh mesh;

        /// <summary> メインスレッドのみ </summary>
        internal void SetMesh(MeshData meshData)
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
                meshCollider.enabled = true;
            }
            else
            {
                mesh.Clear();
            }

            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();

            meshCollider.sharedMesh = mesh;
        }

        private void ClearMesh()
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

        private void OnDestroy()
        {
            if (mesh is not null)
            {
                Destroy(mesh);
            }
        }
    }
}
