using System.Collections.Generic;
using UnityEngine;
using DataObject.Block;

namespace DataObject.Chunk
{
    public class ChunkCollider : MonoBehaviour
    {
        private List<BoxCollider> colliders = new List<BoxCollider>();

        public void UpdateCollider(BlockData[] blocks)
        {
            // TODO コンポーネントを再利用したい
            foreach (var collider in colliders)
            {
                Destroy(collider);
            }

            foreach (var block in blocks)
            {
                if (block.IsRenderSkip) continue;

                var collider = gameObject.AddComponent<BoxCollider>();
                colliders.Add(collider);

                collider.center = block.BlockCoordinate.Center;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
