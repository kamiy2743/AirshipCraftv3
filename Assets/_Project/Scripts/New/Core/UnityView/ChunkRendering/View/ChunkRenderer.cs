using UnityEngine;

namespace UnityView.ChunkRendering.View
{
    internal class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        private Mesh mesh;

        internal void SetRootPosition(Vector3 rootPosition)
        {
            transform.position = rootPosition;
        }

        internal void SetMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            if (vertices.Length == 0 && mesh is not null)
            {
                Destroy(mesh);
                mesh = null;
                meshFilter.sharedMesh = null;
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

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
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