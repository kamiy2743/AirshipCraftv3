using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ACv3.Domain.Chunks
{
    public class SnoiseChunkFactory : IChunkFactory
    {
        readonly SnoiseTerrainGenerator snoiseTerrainGenerator;

        internal SnoiseChunkFactory()
        {
            snoiseTerrainGenerator = new SnoiseTerrainGenerator(128, 0.01f);
        }

        public Chunk Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var pivot = chunkGridCoordinate.ToPivotCoordinate();

            var job = new CreateTerrainJob
            {
                pivot = (int3)pivot,
                generator = snoiseTerrainGenerator,
                result = new NativeArray<Block>(Chunk.BlocksCount, Allocator.TempJob)
            };

            job.Schedule().Complete();

            var blocks = new ChunkBlocks(job.result.ToArray());
            job.result.Dispose();

            return new Chunk(chunkGridCoordinate, blocks);
        }

        [BurstCompile]
        struct CreateTerrainJob : IJob
        {
            [ReadOnly] internal int3 pivot;
            [ReadOnly] internal SnoiseTerrainGenerator generator;
            internal NativeArray<Block> result;

            public void Execute()
            {
                var seek = 0;

                for (int x = 0; x < Chunk.BlockSide; x++)
                {
                    for (int y = 0; y < Chunk.BlockSide; y++)
                    {
                        for (int z = 0; z < Chunk.BlockSide; z++)
                        {
                            var blockType = generator.GetBlockType(pivot.x + x, pivot.y + y, pivot.z + z);
                            result[seek] = new Block(blockType);
                            seek++;
                        }
                    }
                }
            }
        }
    }
}