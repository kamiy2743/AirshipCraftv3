using UnityEngine;
using UnityView.Render;

namespace UnityView.Players
{
    internal class FocusedBlockOutline : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        internal void SetMesh(MeshData meshData)
        {
            var mesh = meshFilter.mesh;
            mesh.Clear();

            mesh.SetVertices(meshData.vertices);
            mesh.SetTriangles(meshData.triangles, 0);
            mesh.SetUVs(0, meshData.uvs);
            mesh.RecalculateNormals();
        }

        internal void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        internal void SetPivot(Vector3 pivot)
        {
            transform.position = pivot;
        }
    }
}