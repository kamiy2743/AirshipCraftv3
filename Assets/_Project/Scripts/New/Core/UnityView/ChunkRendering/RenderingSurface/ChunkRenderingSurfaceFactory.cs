using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal class ChunkRenderingSurfaceFactory : IChunkRenderingSurfaceFactory
    {
        private IChunkProvider chunkProvider;

        internal ChunkRenderingSurfaceFactory(IChunkProvider chunkProvider)
        {
            this.chunkProvider = chunkProvider;
        }

        public ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var surfaces = CreateBlockRenderingSurfaces(chunkGridCoordinate);
            return new ChunkRenderingSurface(chunkGridCoordinate, surfaces);
        }

        private BlockRenderingSurfaces CreateBlockRenderingSurfaces(ChunkGridCoordinate targetChunkGridCoordinate)
        {
            var surfaces = new BlockRenderingSurfaces();
            var context = new Context(targetChunkGridCoordinate, chunkProvider);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        var blockRenderingSurface = new BlockRenderingSurface();

                        foreach (var surface in DirectionExt.Array)
                        {
                            var adjacentBlock = GetAdjacentBlock(surface, rc, context);
                            if (adjacentBlock.blockTypeID == BlockTypeID.Air)
                            {
                                blockRenderingSurface += surface;
                            }
                        }

                        surfaces.SetBlockRenderingSurfaceDirectly(rc, blockRenderingSurface);
                    }
                }
            }

            return surfaces;
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