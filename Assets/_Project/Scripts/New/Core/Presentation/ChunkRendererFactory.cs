using UnityEngine;

namespace Presentation
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