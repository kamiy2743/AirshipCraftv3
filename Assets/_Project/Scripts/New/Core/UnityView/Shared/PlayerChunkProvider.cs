using System;
using UnityEngine;
using Domain;

namespace UnityView.Shared
{
    internal class PlayerChunkProvider
    {
        internal IObservable<ChunkGridCoordinate> OnPlayerChunkChanged;

        private Transform playerTransform;

        internal PlayerChunkProvider()
        {
            playerTransform = Camera.main.transform;
        }

        internal ChunkGridCoordinate GetPlayerChunk()
        {
            return ChunkGridCoordinate.Parse(new BlockGridCoordinate(playerTransform.position));
        }
    }
}