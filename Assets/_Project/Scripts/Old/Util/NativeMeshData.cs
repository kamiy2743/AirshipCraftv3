using UnityEngine;
using Unity.Collections;

namespace Util
{
    public class NativeMeshData
    {
        public readonly NativeArray<Vector3> Vertices;
        public readonly NativeArray<int> Triangles;
        public readonly NativeArray<Vector2> UVs;

        public NativeMeshData(NativeArray<Vector3> vertices, NativeArray<int> triangles, NativeArray<Vector2> uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            UVs = uvs;
        }
    }
}
