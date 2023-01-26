using UnityEngine;

namespace UnityView.ChunkRendering
{
    internal class InSightChecker
    {
        private Camera camera;

        internal InSightChecker()
        {
            camera = Camera.main;
        }

        internal bool Check(Bounds bounds)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
    }
}