using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Domain.Chunks
{
    class SnoiseChunkFactory : IChunkFactory
    {
        readonly SnoiseTerrainGenerator _snoiseTerrainGenerator;

        internal SnoiseChunkFactory()
        {
            _snoiseTerrainGenerator = new SnoiseTerrainGenerator(128, 0.01f);
        }

        public Chunk Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var pivot = chunkGridCoordinate.ToPivotCoordinate();

            var job = new CreateTerrainJob
            {
                Pivot = (int3)pivot,
                Generator = _snoiseTerrainGenerator,
                Result = new NativeArray<Block>(Chunk.BlocksCount, Allocator.TempJob)
            };

            job.Schedule().Complete();

            var blocks = new ChunkBlocks(job.Result.ToArray());
            job.Result.Dispose();

            return new Chunk(chunkGridCoordinate, blocks);
        }

        [BurstCompile]
        struct CreateTerrainJob : IJob
        {
            [ReadOnly] internal int3 Pivot;
            [ReadOnly] internal SnoiseTerrainGenerator Generator;
            internal NativeArray<Block> Result;

            public void Execute()
            {
                var seek = 0;

                for (int x = 0; x < Chunk.BlockSide; x++)
                {
                    for (int y = 0; y < Chunk.BlockSide; y++)
                    {
                        for (int z = 0; z < Chunk.BlockSide; z++)
                        {
                            var blockType = Generator.GetBlockType(Pivot.x + x, Pivot.y + y, Pivot.z + z);
                            Result[seek] = new Block(blockType);
                            seek++;
                        }
                    }
                }
            }
        }
    }
}