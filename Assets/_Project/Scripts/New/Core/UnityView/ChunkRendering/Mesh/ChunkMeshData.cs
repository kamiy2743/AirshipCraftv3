using UnityEngine;

namespace UnityView.ChunkRendering.Mesh
{
    internal record ChunkMeshData
    {
        internal Vector3 rootPosition;
        internal readonly Vector3[] vertices;
        internal readonly int[] triangles;
        internal readonly Vector2[] uvs;

        internal ChunkMeshData(Vector3 rootPosition, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            this.rootPosition = rootPosition;
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}