namespace WorldSystem
{
    public static class WorldSettings
    {
        /// <summary>ワールドの横の長さ(チャンク換算)</summary>
        public const int WorldChunkSideXZ = 7;
        /// <summary>ワールドの縦の長さ(チャンク換算)</summary>
        public const int WorldChunkSideY = 7;

        /// <summary>プレイヤーの周りの読み込み距離(チャンク換算)</summary>
        public const int LoadChunkRadius = 3;

        /// <summary>チャンク内を満たすブロックの立方体の一辺の長さ</summary>
        public const int LocalBlockSide = 16;

        /// <summary>ワールドの横の長さ(ブロック換算)</summary>
        public const int WorldBlockSideXZ = WorldChunkSideXZ * LocalBlockSide;
        /// <summary>ワールドの縦の長さ(ブロック換算)</summary>
        public const int WorldBlockSideY = WorldChunkSideY * LocalBlockSide;
    }
}
