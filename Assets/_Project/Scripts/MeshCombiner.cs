using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace BlockSystem
{
    /// <summary>
    /// メッシュをバッチングする
    /// </summary>
    public class MeshCombiner
    {
        private List<Vector3> batchedVertices = new List<Vector3>();
        private List<int> batchedTriangles = new List<int>();

        public void AddMeshData(MeshData meshData, Vector3 position)
        {
            // triangleはインデックスのため、現在の頂点数を加算しないといけない
            var vc = batchedVertices.Count;
            foreach (var t in meshData.Triangles)
            {
                batchedTriangles.Add(t + vc);
            }

            foreach (var v in meshData.Vertices)
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
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(batchedVertices);
            mesh.SetTriangles(batchedTriangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
