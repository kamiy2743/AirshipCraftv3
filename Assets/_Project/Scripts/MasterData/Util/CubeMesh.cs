using UnityEngine;

namespace MasterData.Util
{
    public static class CubeMesh
    {
        public static readonly Vector3[] Vertices = {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
        };

        public static readonly int[] Triangles = {
            // left
            3, 2, 6,
            6, 7, 3, 
            // right
            4, 5, 1,
            1, 0, 4,
            // top
            1, 5, 6,
            6, 2, 1, 
            // bottom
            4, 0, 3,
            3, 7, 4,
            // forward
            0, 1, 2,
            2, 3, 0, 
            // back
            7, 6, 5,
            5, 4, 7
        };
    }
}
