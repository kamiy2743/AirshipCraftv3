using ACv3.Domain;
using UnityEngine;

namespace ACv3.Presentation.Rendering.Chunks
{
    record ChunkMesh
    {
        internal ChunkGridCoordinate chunkGridCoordinate;
        internal readonly Vector3[] vertices;
        internal readonly int[] triangles;
        internal readonly Vector2[] uvs;

        internal ChunkMesh(ChunkGridCoordinate chunkGridCoordinate, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}