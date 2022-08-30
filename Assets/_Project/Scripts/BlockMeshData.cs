using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData.Block
{
    public class BlockMeshData
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;

        public BlockMeshData(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}
