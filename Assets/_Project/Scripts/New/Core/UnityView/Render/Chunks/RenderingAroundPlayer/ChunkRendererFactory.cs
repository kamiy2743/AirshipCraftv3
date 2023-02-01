using UnityEngine;
using Domain;

namespace UnityView.Render.Chunks
{
    internal class ChunkRendererFactory : MonoBehaviour
    {
        [SerializeField] private ChunkRenderer chunkRendererPrefab;
        [SerializeField] private Transform parent;

        internal ChunkRenderer Create()
        {
            return Instantiate(chunkRendererPrefab, parent: parent);
        }
    }
}