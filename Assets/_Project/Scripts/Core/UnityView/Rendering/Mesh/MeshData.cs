using UnityEngine;

namespace UnityView.Rendering
{
    record MeshData
    {
        internal readonly Vector3[] Vertices;
        internal readonly int[] Triangles;
        internal readonly Vector2[] Uvs;

        internal MeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            Uvs = uvs;
        }
    }
}