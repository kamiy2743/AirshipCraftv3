using UnityEngine;
using System.Collections.Generic;

namespace Util
{
    public static class CubeMesh
    {
        // 外部から書き換え可能だけど、Mesh関係はほとんどVector3[]が引数なのでIReadOnlyCollectionで公開すると扱いが面倒
        public static readonly Vector3[] Vertices = {
            // right
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            // left
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 0),
            // top
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0),
            // bottom
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            // forward
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1),
            new Vector3(0, 0, 1),
            // back
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0)
        };

        public static readonly int[] Triangles = {
            // right
            0, 1, 2,
            2, 3, 0, 
            // left 
            4, 5, 6,
            6, 7, 4, 
            // top
            8, 9, 10,
            10, 11, 8, 
            // bottom
            12, 13, 14,
            14, 15, 12, 
            // forward
            16, 17, 18,
            18, 19, 16, 
            // back
            20, 21, 22,
            22, 23, 20
        };
    }
}
