using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;

namespace BlockSystem
{
    public class ChunkData
    {
        public ChunkCoordinate ChunkCoordinate { get; private set; }

        public IReadOnlyCollection<BlockData> Blocks => _blocks;
        private BlockData[] _blocks;

        public ChunkMeshData ChunkMeshData { get; private set; }

        public ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            _blocks = new BlockData[World.BlockCountInChunk];

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

        private void SetBlockData(LocalCoordinate lc, BlockData blockData)
        {
            _blocks[ToIndex(lc)] = blockData;
        }

        public BlockData GetBlockData(LocalCoordinate lc)
        {
            return _blocks[ToIndex(lc)];
        }

        public void SetChunkMeshData(ChunkMeshData chunkMeshData)
        {
            ChunkMeshData = chunkMeshData;
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
