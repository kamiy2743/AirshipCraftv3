using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    record BlockBounds
    {
        internal readonly float3 Center;
        internal readonly float3 Size;

        BlockBounds(float3 center, float3 size)
        {
            Center = center;
            Size = size;
        }

        internal static BlockBounds CreateCubeBounds(float3 pivot)
        {
            var size = new float3(1, 1, 1);
            var center = pivot + (size * 0.5f);
            return new BlockBounds(center, size);
        }
    }
}