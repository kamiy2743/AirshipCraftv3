using System;
using System.Linq;
using System.Collections.Generic;
using DataObject.Block;
using BlockBehaviour.Interface;
using ChunkConstruction;
using DataStore;
using Unity.Mathematics;
using Unity.Collections;
using BlockOperator;
using UnityEngine;
using UniRx;
using Util;

namespace BlockBehaviour
{
    internal class MUCore : IBlockBehaviour, IInteractedBehaviour, IDisposable
    {
        private BlockDataAccessor _blockDataAccessor;
        private BlockDataUpdater _blockDataUpdater;
        private MeshCombiner _meshCombiner;
        private MURenderer _muRendererPrefab;
        private MUCollider _muColliderPrefab;

        private object syncObject = new object();
        private NativeList<Vector3> verticesBuffer = new NativeList<Vector3>(Allocator.Persistent);
        private NativeList<int> trianglesBuffer = new NativeList<int>(Allocator.Persistent);
        private NativeList<Vector2> uvsBuffer = new NativeList<Vector2>(Allocator.Persistent);

        internal MUCore(BlockDataAccessor blockDataAccessor, BlockDataUpdater blockDataUpdater, MeshCombiner meshCombiner, MURenderer muRendererPrefab, MUCollider muColliderPrefab)
        {
            _blockDataAccessor = blockDataAccessor;
            _blockDataUpdater = blockDataUpdater;
            _meshCombiner = meshCombiner;
            _muRendererPrefab = muRendererPrefab;
            _muColliderPrefab = muColliderPrefab;
        }

        public void OnInteracted(BlockData targetBlockData)
        {
            var chainedBlocks = GetChainedBlocks(targetBlockData, 4096);

            // Airに置換
            var updateBlocks = chainedBlocks.Select(chainedBlock => new BlockData(BlockID.Air, chainedBlock.BlockCoordinate));
            // TODO ctを渡す
            _blockDataUpdater.UpdateBlockData(updateBlocks, default);

            var muData = new MUData(chainedBlocks);
            var muRenderer = MonoBehaviour.Instantiate<MURenderer>(_muRendererPrefab);
            var muCollider = MonoBehaviour.Instantiate<MUCollider>(_muColliderPrefab);

            OnBlockUpdate();

            muData.OnBlockUpdate
                .Subscribe(_ => OnBlockUpdate());

            void OnBlockUpdate()
            {
                lock (syncObject)
                {
                    var blocks = muData.Blocks;

                    // TODO ctを渡す
                    _meshCombiner.Combine(
                        blocks,
                        verticesBuffer,
                        trianglesBuffer,
                        uvsBuffer,
                        default);

                    muRenderer.SetMesh(new NativeMeshData(verticesBuffer, trianglesBuffer, uvsBuffer));
                    muCollider.UpdateCollider(blocks);
                }
            }
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

        public void Dispose()
        {
            verticesBuffer.Dispose();
            trianglesBuffer.Dispose();
            uvsBuffer.Dispose();
        }
    }
}
