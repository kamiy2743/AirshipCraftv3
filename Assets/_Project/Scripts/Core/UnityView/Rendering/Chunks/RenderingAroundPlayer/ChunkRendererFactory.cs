using UnityEngine;
using Domain;

namespace UnityView.Rendering.Chunks
{
    class ChunkRendererFactory : MonoBehaviour
    {
        [SerializeField] ChunkRenderer chunkRendererPrefab;
        [SerializeField] Transform parent;

        internal ChunkRenderer Create()
        {
            return Instantiate(chunkRendererPrefab, parent: parent);
        }
    }
}