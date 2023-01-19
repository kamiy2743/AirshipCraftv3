using Domain;
using Domain.Chunks;

namespace RenderingOptimization
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

                        foreach (var surface in SurfaceExt.Array)
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

        private Block GetAdjacentBlock(Surface surface, RelativeCoordinate rc, Context context)
        {
            var adjacentRelativeCoordinate = rc.Add(surface.ToInt3());

            switch (surface)
            {
                case Surface.Right:
                    if (rc.x == RelativeCoordinate.Max) return context.RightChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Surface.Left:
                    if (rc.x == RelativeCoordinate.Min) return context.LeftChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Surface.Top:
                    if (rc.y == RelativeCoordinate.Max) return context.TopChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Surface.Bottom:
                    if (rc.y == RelativeCoordinate.Min) return context.BottomChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Surface.Forward:
                    if (rc.z == RelativeCoordinate.Max) return context.ForwardChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Surface.Back:
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
                RightChunk = GetAdjacentChunk(Surface.Right, targetChunkGridCoordinate, chunkProvider);
                LeftChunk = GetAdjacentChunk(Surface.Left, targetChunkGridCoordinate, chunkProvider);
                TopChunk = GetAdjacentChunk(Surface.Top, targetChunkGridCoordinate, chunkProvider);
                BottomChunk = GetAdjacentChunk(Surface.Bottom, targetChunkGridCoordinate, chunkProvider);
                ForwardChunk = GetAdjacentChunk(Surface.Forward, targetChunkGridCoordinate, chunkProvider);
                BackChunk = GetAdjacentChunk(Surface.Back, targetChunkGridCoordinate, chunkProvider);
            }

            private Chunk GetAdjacentChunk(Surface surface, ChunkGridCoordinate source, IChunkProvider chunkProvider)
            {
                if (!source.TryAdd(surface.ToInt3(), out var adjacentChunkGridCoordinate))
                {
                    // TODO ワールド端の処理
                    return null;
                }

                return chunkProvider.GetChunk(adjacentChunkGridCoordinate);
            }
        }
    }
}