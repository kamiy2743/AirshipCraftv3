using Util;
using MasterData.Block;

namespace BlockSystem
{
    public struct BlockData
    {
        public readonly BlockID ID;
        public readonly BlockCoordinate BlockCoordinate;

        private SurfaceNormal contactOtherBlockSurfaces;
        public bool IsContactAir => !contactOtherBlockSurfaces.IsFull();

        public BlockData(BlockID id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
            contactOtherBlockSurfaces = SurfaceNormal.None;
        }

        public void SetContactOtherBlockSurfaces(SurfaceNormal surfaces)
        {
            contactOtherBlockSurfaces = surfaces;
        }

        public bool IsContactOtherBlock(SurfaceNormal surface)
        {
            return contactOtherBlockSurfaces.Contains(surface);
        }
    }
}
