using UnityEngine;
using Domain;

namespace UnityView.Rendering.Chunks
{
    record ChunkMesh
    {
        internal readonly ChunkGridCoordinate ChunkGridCoordinate;
        internal readonly Vector3[] Vertices;
        internal readonly int[] Triangles;
        internal readonly Vector2[] Uvs;

        internal ChunkMesh(ChunkGridCoordinate chunkGridCoordinate, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            ChunkGridCoordinate = chunkGridCoordinate;
            Vertices = vertices;
            Triangles = triangles;
            Uvs = uvs;
        }
    }
}