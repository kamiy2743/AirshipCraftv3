using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class MeshData
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;
        public readonly Vector2[] UVs;

        public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            UVs = uvs;
        }
    }
}
