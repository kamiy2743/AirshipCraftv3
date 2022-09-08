using System.Collections.Generic;
using UnityEngine;
using MasterData.Block;
using Util;

namespace BlockSystem
{
    public class ChunkMeshData
    {
        public bool IsEmpty => batchedVertices.Count == 0;
        public Mesh Mesh => _mesh ??= ToMesh();
        private Mesh _mesh;

        private List<Vector3> batchedVertices;
        private List<int> batchedTriangles;

        public ChunkMeshData(int maxVerticesCount, int maxTrianglesCount)
        {
            batchedVertices = new List<Vector3>(maxVerticesCount);
            batchedTriangles = new List<int>(maxTrianglesCount);
        }

        public void AddBlock(BlockData blockData)
        {
            if (_mesh != null)
            {
                throw new System.Exception("Mesh作成済みのためBlockを追加することはできません");
            }

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

            var blockCoordinate = blockData.BlockCoordinate.ToVector3();
            for (int i = 0; i < meshData.Vertices.Length; i++)
            {
                batchedVertices.Add(meshData.Vertices[i] + blockCoordinate);
            }
        }

        /// <summary>
        /// バッチングしたメッシュを新規作成する
        /// </summary>
        private Mesh ToMesh()
        {
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(batchedVertices);
            mesh.SetTriangles(batchedTriangles, 0);
            mesh.RecalculateNormals();

            batchedVertices.Clear();
            batchedTriangles.Clear();

            return mesh;
        }
    }
}
