using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// メッシュをバッチングする
    /// </summary>
    public class MeshCombiner
    {
        private List<Vector3> batchedVertices = new List<Vector3>();
        private List<int> batchedTriangles = new List<int>();

        public void AddMeshData(Vector3[] vertices, int[] triangles, Vector3 position)
        {
            // triangleはインデックスのため、現在の頂点数を加算しないといけない
            var vc = batchedVertices.Count;
            foreach (var t in triangles)
            {
                batchedTriangles.Add(t + vc);
            }

            foreach (var v in vertices)
            {
                batchedVertices.Add(v + position);
            }
        }

        /// <summary>
        /// すべてのメッシュデータをバッチングしたメッシュを作成する
        /// </summary>
        public Mesh Combine()
        {
            var mesh = new Mesh();
            mesh.SetVertices(batchedVertices);
            mesh.SetTriangles(batchedTriangles, 0);
            return mesh;
        }
    }
}
