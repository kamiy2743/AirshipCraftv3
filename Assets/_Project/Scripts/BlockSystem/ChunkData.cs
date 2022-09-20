using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using MasterData.Block;

namespace BlockSystem
{
    internal class ChunkData
    {
        internal ChunkCoordinate ChunkCoordinate { get; private set; }

        internal BlockData[] Blocks;

        internal ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            Blocks = new BlockData[World.BlockCountInChunk];

            var job = new CreateBlockDataJob
            {
                chunkRoot = new int3(cc.x, cc.y, cc.z) * World.ChunkBlockSide,
                mapGenerator = mapGenerator,
                blockDataArray = new NativeArray<BlockData>(World.BlockCountInChunk, Allocator.TempJob)
            };

            job.Schedule(World.BlockCountInChunk, 0).Complete();

            for (int i = 0; i < World.BlockCountInChunk; i++)
            {
                var blockData = job.blockDataArray[i];
                var lc = LocalCoordinate.FromBlockCoordinate(blockData.BlockCoordinate);
                SetBlockData(lc, blockData);
            }

            job.blockDataArray.Dispose();
        }

        private int ToIndex(LocalCoordinate lc)
        {
            return (lc.y * World.ChunkBlockSide * World.ChunkBlockSide) + (lc.z * World.ChunkBlockSide) + lc.x;
        }

        internal void SetBlockData(LocalCoordinate lc, BlockData blockData)
        {
            Blocks[ToIndex(lc)] = blockData;
        }

        internal BlockData GetBlockData(LocalCoordinate lc)
        {
            return Blocks[ToIndex(lc)];
        }

        /// <summary>
        /// コンストラクタ用
        /// BLockDataを生成する
        /// </summary>
        [BurstCompile]
        private struct CreateBlockDataJob : IJobParallelFor
        {
            public int3 chunkRoot;
            public MapGenerator mapGenerator;
            public NativeArray<BlockData> blockDataArray;

            public void Execute(int index)
            {
                var localY = index / (World.ChunkBlockSide * World.ChunkBlockSide);
                var xz = index - (localY * (World.ChunkBlockSide * World.ChunkBlockSide));
                var localX = xz % World.ChunkBlockSide;
                var localZ = xz / World.ChunkBlockSide;

                var localCoordinate = new int3(localX, localY, localZ);
                var blockCoordinate = localCoordinate + chunkRoot;
                var blockID = mapGenerator.GetBlockID(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z);

                var blockData = new BlockData(blockID, new BlockCoordinate(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z));
                blockDataArray[index] = blockData;
            }
        }

    }
}
