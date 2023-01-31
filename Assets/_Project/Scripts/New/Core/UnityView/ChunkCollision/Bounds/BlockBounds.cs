using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    internal record BlockBounds
    {
        internal readonly float3 center;
        internal readonly float3 size;

        internal BlockBounds(float3 center, float3 size)
        {
            this.center = center;
            this.size = size;
        }

        internal static BlockBounds CreateCubeBounds(float3 pivot)
        {
            var size = new float3(1, 1, 1);
            var center = pivot + (size * 0.5f);
            return new BlockBounds(center, size);
        }
    }
}