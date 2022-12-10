using System.Linq;
using System.Collections.Generic;
using DataObject.Block;
using BlockBehaviour.Interface;
using DataStore;
using Unity.Mathematics;
using BlockOperator;
using UnityEngine;
using Util;

namespace BlockBehaviour
{
    internal class MUCore : IBlockBehaviour, IInteractedBehaviour
    {
        private BlockDataAccessor _blockDataAccessor;
        private BlockDataUpdater _blockDataUpdater;
        private MUCoreObject _muCoreObjectPrefab;

        internal MUCore(BlockDataAccessor blockDataAccessor, BlockDataUpdater blockDataUpdater, MUCoreObject muCoreObjectPrefab)
        {
            _blockDataAccessor = blockDataAccessor;
            _blockDataUpdater = blockDataUpdater;
            _muCoreObjectPrefab = muCoreObjectPrefab;
        }

        public void OnInteracted(BlockData targetBlockData)
        {
            var chainedBlocks = GetChainedBlocks(targetBlockData, 4096);

            // Airに置換
            var updateBlocks = chainedBlocks.Select(chainedBlock => new BlockData(BlockID.Air, chainedBlock.BlockCoordinate));
            // TODO ctを渡す
            _blockDataUpdater.UpdateBlockData(updateBlocks, default);

            var muCoreObject = MonoBehaviour.Instantiate<MUCoreObject>(
                _muCoreObjectPrefab,
                targetBlockData.BlockCoordinate.Center,
                Quaternion.identity);
            muCoreObject.SetMesh(CubeMesh.MeshData);
        }

        private HashSet<BlockData> GetChainedBlocks(BlockData targetBlock, int maxCount)
        {
            var chainedBlocksCount = 0;
            var chainedBlocks = new HashSet<BlockData>();
            var chainedBlockCoordinates = new HashSet<int3>();
            var searchBlockQueue = new Queue<BlockData>();
            searchBlockQueue.Enqueue(targetBlock);

            while (searchBlockQueue.TryDequeue(out var startBlock))
            {
                var startPosition = startBlock.BlockCoordinate.ToInt3();

                for (int x = startPosition.x - 1; x <= startPosition.x + 1; x++)
                {
                    for (int y = startPosition.y - 1; y <= startPosition.y + 1; y++)
                    {
                        for (int z = startPosition.z - 1; z <= startPosition.z + 1; z++)
                        {
                            if (chainedBlocksCount >= maxCount) break;
                            if (chainedBlockCoordinates.Contains(new int3(x, y, z))) continue;

                            // TODO ctを渡す
                            var blockData = _blockDataAccessor.GetBlockData(x, y, z, default);

                            if (blockData == BlockData.Empty) continue;
                            if (blockData.ID == BlockID.Air) continue;

                            var firstAdd = chainedBlocks.Add(blockData);
                            if (firstAdd)
                            {
                                chainedBlocksCount++;
                                searchBlockQueue.Enqueue(blockData);
                                chainedBlockCoordinates.Add(blockData.BlockCoordinate.ToInt3());
                            }
                        }
                    }
                }
            }

            return chainedBlocks;
        }
    }
}
