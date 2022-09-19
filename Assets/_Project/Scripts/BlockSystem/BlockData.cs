using Util;
using MasterData.Block;

namespace BlockSystem
{
    public struct BlockData
    {
        public readonly BlockID ID;
        public readonly BlockCoordinate BlockCoordinate;

        private SurfaceNormal contactOtherBlockSurfaces;
        internal bool IsContactAir => !contactOtherBlockSurfaces.IsFull();

        internal BlockData(BlockID id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
            contactOtherBlockSurfaces = SurfaceNormal.None;
        }

        internal void SetContactOtherBlockSurfaces(SurfaceNormal surfaces)
        {
            contactOtherBlockSurfaces = surfaces;
        }

        internal bool IsContactOtherBlock(SurfaceNormal surface)
        {
            return contactOtherBlockSurfaces.Contains(surface);
        }
    }
}
