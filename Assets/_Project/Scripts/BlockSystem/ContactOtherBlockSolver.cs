using Util;
using MasterData.Block;

namespace BlockSystem
{
    /// <summary>
    /// 他のブロックと接している面を取得する
    /// </summary>
    internal class ContactOtherBlockSolver
    {
        private ChunkDataStore _chunkDataStore;

        internal ContactOtherBlockSolver(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        internal SurfaceNormal GetContactOtherBlockSurfaces(BlockCoordinate bc)
        {
            var surfaces = SurfaceNormal.Zero;

            foreach (var surface in SurfaceNormalExt.List)
            {
                if (IsContactOtherBlock(surface, bc))
                {
                    surfaces = surfaces.Add(surface);
                }
            }

            return surfaces;
        }

        private bool IsContactOtherBlock(SurfaceNormal surface, BlockCoordinate bc)
        {
            var checkCoordinate = bc.ToVector3() + surface.ToVector3();
            if (BlockCoordinate.IsValid(checkCoordinate))
            {
                if (IsAir(new BlockCoordinate(checkCoordinate)))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAir(BlockCoordinate bc)
        {
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);
            var chunkData = _chunkDataStore.GetChunkData(cc);
            var blockData = chunkData.GetBlockData(lc);
            return blockData.ID == BlockID.Air;
        }
    }
}
