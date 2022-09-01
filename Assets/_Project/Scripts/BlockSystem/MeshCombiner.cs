using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterData.Block;
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

        public void AddBlock(BlockData blockData)
        {
            var meshData = MasterBlockDataStore.GetData(blockData.ID).MeshData;
            if (meshData.Vertices.Length == 0) return;

            // triangleはインデックスのため、現在の頂点数を加算しないといけない
            var vc = batchedVertices.Count;

            // CubeMeshじゃないと動かない
            for (int i = 0; i < 6; i++)
            {
                // 他のブロックに面していれば描画しない
                if (blockData.IsContactOtherBlock((SurfaceNormal)i))
                {
                    continue;
                }

                for (int j = 0; j < 6; j++)
                {
                    var t = meshData.Triangles[i * 6 + j];
                    batchedTriangles.Add(t + vc);
                }
            }

            foreach (var v in meshData.Vertices)
            {
                batchedVertices.Add(v + blockData.BlockCoordinate.ToVector3());
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
