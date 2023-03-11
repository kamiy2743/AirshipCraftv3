using UnityEngine;

namespace UnityView.ChunkCollision
{
    class ChunkColliderFactory : MonoBehaviour
    {
        [SerializeField] ChunkCollider chunkColliderPrefab;
        [SerializeField] Transform parent;

        internal ChunkCollider Create()
        {
            return Instantiate(chunkColliderPrefab, parent: parent);
        }
    }
}