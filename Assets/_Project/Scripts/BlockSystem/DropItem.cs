using UnityEngine;
using Util;

namespace BlockSystem
{
    internal class DropItem : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        internal void SetPosition(Vector3 position)
        {
            transform.position = position - (transform.localScale * 0.5f);
        }

        internal void SetMesh(MeshData meshData)
        {
            var mesh = meshFilter.mesh;
            mesh.Clear();

            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();
        }
    }
}
