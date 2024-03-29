using UnityEngine;

namespace ACv3.Presentation.ChunkCollision
{
    public class ChunkColliderFactory : MonoBehaviour
    {
        [SerializeField] ChunkCollider chunkColliderPrefab;
        [SerializeField] Transform parent;

        internal ChunkCollider Create()
        {
            return Instantiate(chunkColliderPrefab, parent: parent);
        }
    }
}