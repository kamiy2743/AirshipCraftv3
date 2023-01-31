using UnityEngine;

namespace UnityView.ChunkCollision
{
    internal class ChunkColliderFactory : MonoBehaviour
    {
        [SerializeField] private ChunkCollider chunkColliderPrefab;
        [SerializeField] private Transform parent;

        internal ChunkCollider Create()
        {
            return Instantiate(chunkColliderPrefab, parent: parent);
        }
    }
}