using UnityEngine;

namespace UnityView.ChunkRender.Mesh
{
    internal record ChunkMeshData
    {
        internal Vector3 pivotCoordinate;
        internal readonly Vector3[] vertices;
        internal readonly int[] triangles;
        internal readonly Vector2[] uvs;

        internal ChunkMeshData(Vector3 pivotCoordinate, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            this.pivotCoordinate = pivotCoordinate;
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}