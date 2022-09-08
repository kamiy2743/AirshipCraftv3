using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace BlockSystem
{
    /// <summary>
    /// 他のブロックと接している面を取得する
    /// </summary>
    public class ContactOtherBlockSolver
    {
        private ChunkDataStore _chunkDataStore;

        public ContactOtherBlockSolver(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        public ContactSurfaces GetContactOtherBlockSurfaces(BlockCoordinate bc)
        {
            var surfaces = new ContactSurfaces();

            foreach (var surface in SurfaceNormalExt.List)
            {
                if (IsContactOtherBlock(surface, bc))
                {
                    surfaces.Add(surface);
                }
            }

            return surfaces;
        }

        private bool IsAir(BlockCoordinate bc)
        {
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);
            var chunkData = _chunkDataStore.GetChunkData(cc);
            var blockData = chunkData.GetBlockData(lc);
            return blockData.ID == 0;
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
    }
}
