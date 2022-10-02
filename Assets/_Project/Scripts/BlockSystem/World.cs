using UnityEngine;

namespace BlockSystem
{
    internal class World
    {
        /// <summary>ワールドの横の長さ(チャンク換算)</summary>
        internal const int WorldChunkSideXZ = 256;
        /// <summary>ワールドの縦の長さ(チャンク換算)</summary>
        internal const int WorldChunkSideY = 4;

        /// <summary>プレイヤーの周りの読み込み距離(チャンク換算)</summary>
        internal static readonly int LoadChunkRadius = 4;
        internal static readonly int LoadChunkCount = (int)Mathf.Pow(LoadChunkRadius * 2 + 1, 3);

        /// <summary>チャンク内を満たすブロックの立方体の一辺の長さ</summary>
        internal const int ChunkBlockSide = 16;
        /// <summary>チャンク内のブロックの総数</summary>
        internal const int BlockCountInChunk = ChunkBlockSide * ChunkBlockSide * ChunkBlockSide;

        /// <summary>ワールドの横の長さ(ブロック換算)</summary>
        internal const int WorldBlockSideXZ = WorldChunkSideXZ * ChunkBlockSide;
        /// <summary>ワールドの縦の長さ(ブロック換算)</summary>
        internal const int WorldBlockSideY = WorldChunkSideY * ChunkBlockSide;
    }
}
