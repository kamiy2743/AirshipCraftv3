using System.Collections.Generic;
using UnityEngine;
using MasterData.Block;
using Util;

namespace BlockSystem
{
    public class ChunkMeshData
    {
        public List<Vector3> Vertices;
        public List<int> Triangles;
        public List<Vector2> UVs;

        public bool IsEmpty => Vertices.Count == 0;

        private static readonly float uvSide = Mathf.CeilToInt(Mathf.Sqrt(BlockID.Max + 1));
        private static readonly float uvUnit = 1 / uvSide;

        public ChunkMeshData(int maxVerticesCount, int maxTrianglesCount, int maxUVsCount)
        {
            Vertices = new List<Vector3>(maxVerticesCount);
            Triangles = new List<int>(maxTrianglesCount);
            UVs = new List<Vector2>(maxUVsCount);
        }

        public void AddBlock(BlockData blockData)
        {
            if (blockData.ID.IsAir) return;

            var meshData = MasterBlockDataStore.GetData(blockData.ID.value).MeshData;

            // triangleはインデックスのため、現在の頂点数を加算しないといけない
            var vc = Vertices.Count;

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
                    Triangles.Add(t + vc);
                }
            }


            // CubeMeshじゃないと動かない
            // 6面に同じuvを設定
            var uvX = (int)(blockData.ID.value % uvSide);
            var uvY = (int)(blockData.ID.value / uvSide);
            for (int i = 0; i < 6; i++)
            {
                UVs.Add(new Vector2(uvX, uvY) * uvUnit);
                UVs.Add(new Vector2(uvX, uvY + 1) * uvUnit);
                UVs.Add(new Vector2(uvX + 1, uvY + 1) * uvUnit);
                UVs.Add(new Vector2(uvX + 1, uvY) * uvUnit);
            }

            var blockCoordinate = blockData.BlockCoordinate.ToVector3();
            for (int i = 0; i < meshData.Vertices.Length; i++)
            {
                Vertices.Add(meshData.Vertices[i] + blockCoordinate);
            }
        }

        public void Clear()
        {
            Vertices.Clear();
            Triangles.Clear();
            UVs.Clear();
        }
    }
}
