using Domain;
using Domain.Chunks;

namespace UnityView.Rendering.Chunks
{
    internal class ChunkSurfaceFactory
    {
        private IChunkProvider chunkProvider;

        internal ChunkSurfaceFactory(IChunkProvider chunkProvider)
        {
            this.chunkProvider = chunkProvider;
        }

        internal ChunkSurface Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var chunkSurface = new ChunkSurface(chunkGridCoordinate);
            var context = new Context(chunkGridCoordinate, chunkProvider);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        var blockSurface = new BlockSurface();

                        var blockType = context.TargetChunk.GetBlock(rc).blockType;
                        if (blockType == BlockType.Air)
                        {
                            continue;
                        }

                        foreach (var direction in DirectionExt.Array)
                        {
                            var adjacentBlock = GetAdjacentBlock(direction, rc, context);
                            if (adjacentBlock.blockType == BlockType.Air)
                            {
                                blockSurface += direction;
                            }
                        }

                        chunkSurface.SetBlockSurfaceDirectly(rc, blockSurface);
                    }
                }
            }

            return chunkSurface;
        }

        private Block GetAdjacentBlock(Direction direction, RelativeCoordinate rc, Context context)
        {
            var adjacentRelativeCoordinate = rc.Add(direction.ToInt3());

            switch (direction)
            {
                case Direction.Right:
                    if (rc.x == RelativeCoordinate.Max) return context.RightChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Left:
                    if (rc.x == RelativeCoordinate.Min) return context.LeftChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Top:
                    if (rc.y == RelativeCoordinate.Max) return context.TopChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Bottom:
                    if (rc.y == RelativeCoordinate.Min) return context.BottomChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Forward:
                    if (rc.z == RelativeCoordinate.Max) return context.ForwardChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Back:
                    if (rc.z == RelativeCoordinate.Min) return context.BackChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
            }

            return context.TargetChunk.GetBlock(adjacentRelativeCoordinate);
        }

        private class Context
        {
            internal Chunk TargetChunk;
            internal Chunk RightChunk;
            internal Chunk LeftChunk;
            internal Chunk TopChunk;
            internal Chunk BottomChunk;
            internal Chunk ForwardChunk;
            internal Chunk BackChunk;

            internal Context(ChunkGridCoordinate targetChunkGridCoordinate, IChunkProvider chunkProvider)
            {
                TargetChunk = chunkProvider.GetChunk(targetChunkGridCoordinate);
                RightChunk = GetAdjacentChunk(Direction.Right, targetChunkGridCoordinate, chunkProvider);
                LeftChunk = GetAdjacentChunk(Direction.Left, targetChunkGridCoordinate, chunkProvider);
                TopChunk = GetAdjacentChunk(Direction.Top, targetChunkGridCoordinate, chunkProvider);
                BottomChunk = GetAdjacentChunk(Direction.Bottom, targetChunkGridCoordinate, chunkProvider);
                ForwardChunk = GetAdjacentChunk(Direction.Forward, targetChunkGridCoordinate, chunkProvider);
                BackChunk = GetAdjacentChunk(Direction.Back, targetChunkGridCoordinate, chunkProvider);
            }

            private Chunk GetAdjacentChunk(Direction direction, ChunkGridCoordinate source, IChunkProvider chunkProvider)
            {
                if (!source.TryAdd(direction.ToInt3(), out var adjacentChunkGridCoordinate))
                {
                    // TODO ワールド端の処理
                    return null;
                }

                return chunkProvider.GetChunk(adjacentChunkGridCoordinate);
            }
        }
    }
}