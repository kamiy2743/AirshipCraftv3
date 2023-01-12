namespace Domain.Chunks
{
    public record ChunkGridCoordinate
    {
        public readonly short x;
        public readonly short y;
        public readonly short z;

        internal const short Max = short.MaxValue;
        internal const short Min = short.MinValue;

        internal ChunkGridCoordinate(short x, short y, short z)
        {
            // 現状MaxとMinがshort.MaxValueとMinValueに対応しているのでバリデーションはしない
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal static ChunkGridCoordinate Parse(BlockGridCoordinate blockGridCoordinate)
        {
            return new ChunkGridCoordinate(
                (short)(blockGridCoordinate.x >> Chunk.BlockSideShift),
                (short)(blockGridCoordinate.y >> Chunk.BlockSideShift),
                (short)(blockGridCoordinate.z >> Chunk.BlockSideShift)
            );
        }
    }
}