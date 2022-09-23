using System.Collections.Generic;
using UnityEngine;
using MasterData.Block;
using Util;

namespace BlockSystem
{
    internal class ChunkMeshData
    {
        internal List<Vector3> Vertices;
        internal List<int> Triangles;
        internal List<Vector2> UVs;

        internal bool IsEmpty => Vertices.Count == 0;

        internal ChunkMeshData(int maxVerticesCount, int maxTrianglesCount, int maxUVsCount)
        {
            Vertices = new List<Vector3>(maxVerticesCount);
            Triangles = new List<int>(maxTrianglesCount);
            UVs = new List<Vector2>(maxUVsCount);
        }

        internal void AddBlock(BlockData blockData)
        {
            if (blockData.IsRenderSkip) return;

            var meshData = MasterBlockDataStore.GetData(blockData.ID).MeshData;

            // triangleはインデックスのため、現在の頂点数を加算しないといけない
            var vc = Vertices.Count;

            // CubeMeshじゃないと動かない
            for (int i = 0; i < 6; i++)
            {
                // 他のブロックに面していれば描画しない
                if (blockData.IsContactOtherBlock(SurfaceNormalExt.FromIndex(i)))
                {
                    continue;
                }

                for (int j = 0; j < 6; j++)
                {
                    var t = meshData.Triangles[i * 6 + j];
                    Triangles.Add(t + vc);
                }
            }

            UVs.AddRange(meshData.UVs);

            var blockCoordinate = blockData.BlockCoordinate.ToVector3();
            for (int i = 0; i < meshData.Vertices.Length; i++)
            {
                Vertices.Add(meshData.Vertices[i] + blockCoordinate);
            }
        }

        internal void Clear()
        {
            Vertices.Clear();
            Triangles.Clear();
            UVs.Clear();
        }
    }
}
