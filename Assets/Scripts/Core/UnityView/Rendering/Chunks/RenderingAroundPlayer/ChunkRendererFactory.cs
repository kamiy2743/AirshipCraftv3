using UnityEngine;

namespace ACv3.UnityView.Rendering.Chunks
{
    public class ChunkRendererFactory : MonoBehaviour
    {
        [SerializeField] ChunkRenderer chunkRendererPrefab;
        [SerializeField] Transform parent;

        internal ChunkRenderer Create()
        {
            return Instantiate(chunkRendererPrefab, parent: parent);
        }
    }
}