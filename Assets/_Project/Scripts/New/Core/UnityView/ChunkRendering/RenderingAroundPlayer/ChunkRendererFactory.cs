using UnityEngine;
using Domain.Chunks;

namespace UnityView.ChunkRendering
{
    internal class ChunkRendererFactory : MonoBehaviour
    {
        [SerializeField] private ChunkRenderer chunkRendererPrefab;
        [SerializeField] private Transform parent;

        internal ChunkRenderer Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            return Instantiate(chunkRendererPrefab, parent: parent);
        }
    }
}