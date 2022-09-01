using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class MeshData
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;

        public MeshData(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}
