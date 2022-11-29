using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using DataObject.Chunk;

namespace BlockSystem
{
    internal class ChunkColliderSystem : IDisposable
    {
        private ChunkObjectPool _chunkObjectPool;

        private const int MaxEffectiveTime = 600;
        private const int UpdateInterval = 60;

        private IDisposable chunkChangeDisposal;
        private IDisposable updateDisposal;

        private Dictionary<ChunkCoordinate, int> effectiveTimeDictionary = new Dictionary<ChunkCoordinate, int>();

        internal ChunkColliderSystem(PlayerChunkChangeDetector playerChunkChangeDetector, ChunkObjectPool chunkObjectPool)
        {
            _chunkObjectPool = chunkObjectPool;

            chunkChangeDisposal = playerChunkChangeDetector.OnDetect
                .Subscribe(playerChunk =>
                {
                    UpdateAroundPlayer(playerChunk);
                });

            updateDisposal = Observable.EveryUpdate()
                .ThrottleFirstFrame(UpdateInterval)
                .Subscribe(_ =>
                {
                    UpdateAroundPlayer(playerChunkChangeDetector.PlayerChunk);
                    DecreaseEffectiveTime();
                });
        }

        private void UpdateAroundPlayer(int3 playerChunk)
        {
            for (int x = playerChunk.x - World.SimulationRadius; x <= playerChunk.x + World.SimulationRadius; x++)
            {
                for (int y = playerChunk.y - World.SimulationRadius; y <= playerChunk.y + World.SimulationRadius; y++)
                {
                    for (int z = playerChunk.z - World.SimulationRadius; z <= playerChunk.z + World.SimulationRadius; z++)
                    {
                        if (!ChunkCoordinate.IsValid(x, y, z)) return;

                        var cc = new ChunkCoordinate(x, y, z);
                        effectiveTimeDictionary[cc] = MaxEffectiveTime;
                        if (_chunkObjectPool.ChunkObjects.TryGetValue(cc, out var chunkObject))
                        {
                            chunkObject.SetColliderEnabled(true);
                        }
                    }
                }
            }
        }

        private void DecreaseEffectiveTime()
        {
            var chunks = effectiveTimeDictionary.Keys.ToList();
            foreach (var cc in chunks)
            {
                var time = effectiveTimeDictionary[cc] - 1;
                if (time > 0)
                {
                    effectiveTimeDictionary[cc] = time;
                    continue;
                }

                effectiveTimeDictionary.Remove(cc);
                if (_chunkObjectPool.ChunkObjects.TryGetValue(cc, out var chunkObject))
                {
                    chunkObject.SetColliderEnabled(false);
                }
            }
        }

        public void Dispose()
        {
            chunkChangeDisposal.Dispose();
            updateDisposal.Dispose();
        }
    }
}
