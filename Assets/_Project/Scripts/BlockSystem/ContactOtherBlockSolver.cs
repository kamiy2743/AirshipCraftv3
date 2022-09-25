using Util;
using MasterData.Block;
using System.Collections.Generic;

namespace BlockSystem
{
    /// <summary>
    /// 他のブロックと接している面を取得する
    /// </summary>
    internal class ContactOtherBlockSolver
    {
        internal SurfaceNormal GetContactOtherBlockSurfaces(BlockCoordinate targetCoordinate, Dictionary<ChunkCoordinate, ChunkData> chunkDataCache)
        {
            var surfaces = SurfaceNormal.Zero;

            foreach (var surface in SurfaceNormalExt.List)
            {
                var checkCoordinate = targetCoordinate.ToVector3() + surface.ToVector3();
                if (!BlockCoordinate.IsValid(checkCoordinate)) continue;

                var bc = new BlockCoordinate(checkCoordinate);
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = chunkDataCache[cc];
                var blockData = chunkData.GetBlockData(lc);
                if (blockData.ID == BlockID.Air) continue;

                surfaces = surfaces.Add(surface);
            }

            return surfaces;
        }
    }
}
