using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace BlockSystem
{
    /// <summary>
    /// 他のブロックと接している面を取得する
    /// </summary>
    public class ContactOtherBlockSolver : MonoBehaviour
    {
        [SerializeField] private ChunkDataStore chunkDataStore;

        public static ContactOtherBlockSolver Instance => _instance;
        private static ContactOtherBlockSolver _instance;

        void Awake()
        {
            _instance = this;
        }

        public List<SurfaceNormal> GetContactOtherBlockSurfaces(BlockCoordinate bc)
        {
            var surfaces = new List<SurfaceNormal>();

            foreach (SurfaceNormal surface in System.Enum.GetValues(typeof(SurfaceNormal)))
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
            var chunkData = chunkDataStore.GetChunkData(cc);
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
