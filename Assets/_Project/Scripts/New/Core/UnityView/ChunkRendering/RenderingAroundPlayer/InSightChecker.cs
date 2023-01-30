using UnityEngine;
using Unity.Mathematics;

namespace UnityView.ChunkRendering
{
    internal class InSightChecker
    {
        private readonly float4x4 viewportMatrix;

        internal InSightChecker()
        {
            var camera = Camera.main;
            viewportMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
        }

        internal bool Check(Bounds bounds)
        {
            var m = bounds.min;
            var s = bounds.size;

            if (PointCheck(m.x, m.y, m.z)) return true;
            if (PointCheck(m.x, m.y + s.y, m.z)) return true;
            if (PointCheck(m.x, m.y, m.z + s.z)) return true;
            if (PointCheck(m.x, m.y + s.y, m.z + s.z)) return true;
            if (PointCheck(m.x + s.x, m.y, m.z)) return true;
            if (PointCheck(m.x + s.x, m.y + s.y, m.z)) return true;
            if (PointCheck(m.x + s.x, m.y, m.z + s.z)) return true;
            if (PointCheck(m.x + s.x, m.y + s.y, m.z + s.z)) return true;
            return false;
        }

        private bool PointCheck(float x, float y, float z)
        {
            var viewportPoint = WorldToViewportPoint(x, y, z);

            if (viewportPoint.z > 1) return false;
            if (viewportPoint.x > 1 || viewportPoint.x < -1) return false;
            if (viewportPoint.y > 1 || viewportPoint.y < -1) return false;
            return true;
        }

        private float3 WorldToViewportPoint(float x, float y, float z)
        {
            var viewportPoint = Multiply(x, y, z);
            // x,y,zをwで割る
            var div = 1 / viewportPoint.w;
            return new float3(
                viewportPoint.x * div,
                viewportPoint.y * div,
                viewportPoint.z * div);
        }

        // math.mul()よりも速い
        private float4 Multiply(float b1, float b2, float b3)
        {
            return new float4(
                (viewportMatrix.c0.x * b1) + (viewportMatrix.c1.x * b2) + (viewportMatrix.c2.x * b3) + (viewportMatrix.c3.x),
                (viewportMatrix.c0.y * b1) + (viewportMatrix.c1.y * b2) + (viewportMatrix.c2.y * b3) + (viewportMatrix.c3.y),
                (viewportMatrix.c0.z * b1) + (viewportMatrix.c1.z * b2) + (viewportMatrix.c2.z * b3) + (viewportMatrix.c3.z),
                (viewportMatrix.c0.w * b1) + (viewportMatrix.c1.w * b2) + (viewportMatrix.c2.w * b3) + (viewportMatrix.c3.w));
        }
    }
}