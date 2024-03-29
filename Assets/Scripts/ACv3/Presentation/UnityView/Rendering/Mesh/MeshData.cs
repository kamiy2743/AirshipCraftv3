using UnityEngine;

namespace ACv3.Presentation.Rendering
{
    record MeshData
    {
        internal readonly Vector3[] vertices;
        internal readonly int[] triangles;
        internal readonly Vector2[] uvs;

        internal MeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}