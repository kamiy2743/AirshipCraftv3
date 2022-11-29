using System;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using DataObject.Chunk;

namespace BlockSystem
{
    internal class PlayerChunkChangeDetector : IDisposable
    {
        private Transform player;
        internal int3 PlayerChunk => GetPlayerChunk(player.position);

        internal IObservable<int3> OnDetect => _onDetect;
        private Subject<int3> _onDetect = new Subject<int3>();
        private IDisposable updateDisposal;

        internal PlayerChunkChangeDetector(Transform player)
        {
            this.player = player;
            int3 lastPlayerChunk = new int3() * int.MinValue;

            updateDisposal = player
                .ObserveEveryValueChanged(player => player.position)
                .Select(position => GetPlayerChunk(position))
                .Subscribe(playerChunk =>
                {
                    if (!playerChunk.Equals(lastPlayerChunk))
                    {
                        lastPlayerChunk = playerChunk;
                        _onDetect.OnNext(playerChunk);
                    }
                });
        }

        /// <summary> プレイヤーがいるチャンクに変換 </summary>
        private static int3 GetPlayerChunk(Vector3 playerPosition)
        {
            return new int3(
                (int)math.floor(playerPosition.x) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.y) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.z) >> ChunkData.ChunkBlockSideShift
            );
        }

        public void Dispose()
        {
            updateDisposal.Dispose();
        }
    }
}
