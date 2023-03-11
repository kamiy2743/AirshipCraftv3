using UnityEngine;
using UnityView.Rendering;

namespace UnityView.Players
{
    class FocusedBlockOutline : MonoBehaviour
    {
        [SerializeField] MeshFilter meshFilter;

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