namespace BlockSystem
{
    public class World
    {
        /// <summary>ワールドの横の長さ(チャンク換算)</summary>
        public const int WorldChunkSideXZ = 33;
        /// <summary>ワールドの縦の長さ(チャンク換算)</summary>
        public const int WorldChunkSideY = 4;

        /// <summary>プレイヤーの周りの読み込み距離(チャンク換算)</summary>
        public static readonly int LoadChunkRadius = 16;

        /// <summary>チャンク内を満たすブロックの立方体の一辺の長さ</summary>
        public const int ChunkBlockSide = 16;
        /// <summary>チャンク内のブロックの総数</summary>
        public const int BlockCountInChunk = ChunkBlockSide * ChunkBlockSide * ChunkBlockSide;

        /// <summary>ワールドの横の長さ(ブロック換算)</summary>
        public const int WorldBlockSideXZ = WorldChunkSideXZ * ChunkBlockSide;
        /// <summary>ワールドの縦の長さ(ブロック換算)</summary>
        public const int WorldBlockSideY = WorldChunkSideY * ChunkBlockSide;
    }
}
