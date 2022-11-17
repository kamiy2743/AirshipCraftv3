using System;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace BlockSystem
{
    internal class PlayerChunkChangeDetector
    {
        internal readonly IObservable<int3> OnDetect;

        internal PlayerChunkChangeDetector(Transform player)
        {
            int3 lastPlayerChunk = default;
            var isFirst = true;

            OnDetect = player
                .ObserveEveryValueChanged(player => player.position)
                .Select(position => GetPlayerChunk(position))
                .Where(playerChunk =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        return true;
                    }

                    if (!playerChunk.Equals(lastPlayerChunk))
                    {
                        lastPlayerChunk = playerChunk;
                        return true;
                    }

                    return false;
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
    }
}
