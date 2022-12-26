using System.Collections.Generic;
using UnityEngine;
using DataObject.Block;

namespace BlockBehaviour
{
    public class MUCollider : MonoBehaviour
    {
        private List<BoxCollider> colliders = new List<BoxCollider>();

        internal void UpdateCollider(BlockData[] blocks)
        {
            var renderCount = 0;
            var collidersCount = colliders.Count;

            foreach (var block in blocks)
            {
                if (block.IsRenderSkip) continue;

                BoxCollider collider;
                // 作成済みコライダーがあれば再利用
                if (renderCount < collidersCount)
                {
                    collider = colliders[renderCount];
                }
                else
                {
                    collider = gameObject.AddComponent<BoxCollider>();
                    colliders.Add(collider);
                }

                collider.center = block.BlockCoordinate.Center;
                renderCount++;
            }

            // コライダーが余剰なら削除
            if (collidersCount > renderCount)
            {
                for (int i = renderCount; i < collidersCount; i++)
                {
                    Destroy(colliders[i]);
                }

                colliders.RemoveRange(renderCount, collidersCount - renderCount);
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
