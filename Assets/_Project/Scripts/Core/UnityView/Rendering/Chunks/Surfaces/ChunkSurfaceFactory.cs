using System;
using Domain;
using Domain.Chunks;

namespace UnityView.Rendering.Chunks
{
    class ChunkSurfaceFactory
    {
        readonly IChunkProvider _chunkProvider;

        internal ChunkSurfaceFactory(IChunkProvider chunkProvider)
        {
            _chunkProvider = chunkProvider;
        }

        internal ChunkSurface Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var chunkSurface = new ChunkSurface(chunkGridCoordinate);
            var context = new Context(chunkGridCoordinate, _chunkProvider);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        var blockSurface = new BlockSurface();

                        var blockType = context.TargetChunk.GetBlock(rc).BlockType;
                        if (blockType == BlockType.Air)
                        {
                            continue;
                        }

                        foreach (var direction in DirectionExt.Array)
                        {
                            var adjacentBlock = GetAdjacentBlock(direction, rc, context);
                            if (adjacentBlock.BlockType == BlockType.Air)
                            {
                                blockSurface += FaceExt.Parse(direction);
                            }
                        }

                        chunkSurface.SetBlockSurfaceDirectly(rc, blockSurface);
                    }
                }
            }

            return chunkSurface;
        }

        Block GetAdjacentBlock(Direction direction, RelativeCoordinate rc, Context context)
        {
            var adjacentRelativeCoordinate = rc.Add(direction.ToInt3());

            switch (direction)
            {
                case Direction.Right:
                    if (rc.X == RelativeCoordinate.Max) return context.RightChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Left:
                    if (rc.X == RelativeCoordinate.Min) return context.LeftChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Up:
                    if (rc.Y == RelativeCoordinate.Max) return context.TopChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Down:
                    if (rc.Y == RelativeCoordinate.Min) return context.BottomChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.Forward:
                    if (rc.Z == RelativeCoordinate.Max) return context.ForwardChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                case Direction.BackWard:
                    if (rc.Z == RelativeCoordinate.Min) return context.BackwardChunk.GetBlock(adjacentRelativeCoordinate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return context.TargetChunk.GetBlock(adjacentRelativeCoordinate);
        }

        class Context
        {
            internal readonly Chunk TargetChunk;
            internal readonly Chunk RightChunk;
            internal readonly Chunk LeftChunk;
            internal readonly Chunk TopChunk;
            internal readonly Chunk BottomChunk;
            internal readonly Chunk ForwardChunk;
            internal readonly Chunk BackwardChunk;

            internal Context(ChunkGridCoordinate targetChunkGridCoordinate, IChunkProvider chunkProvider)
            {
                TargetChunk = chunkProvider.GetChunk(targetChunkGridCoordinate);
                RightChunk = GetAdjacentChunk(Direction.Right, targetChunkGridCoordinate, chunkProvider);
                LeftChunk = GetAdjacentChunk(Direction.Left, targetChunkGridCoordinate, chunkProvider);
                TopChunk = GetAdjacentChunk(Direction.Up, targetChunkGridCoordinate, chunkProvider);
                BottomChunk = GetAdjacentChunk(Direction.Down, targetChunkGridCoordinate, chunkProvider);
                ForwardChunk = GetAdjacentChunk(Direction.Forward, targetChunkGridCoordinate, chunkProvider);
                BackwardChunk = GetAdjacentChunk(Direction.BackWard, targetChunkGridCoordinate, chunkProvider);
            }

            Chunk GetAdjacentChunk(Direction direction, ChunkGridCoordinate source, IChunkProvider chunkProvider)
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