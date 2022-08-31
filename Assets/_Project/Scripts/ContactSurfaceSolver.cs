using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    public class ContactSurfaceSolver : MonoBehaviour
    {
        [SerializeField] private ChunkDataStore chunkDataStore;

        public static ContactSurfaceSolver Instance => _instance;
        private static ContactSurfaceSolver _instance;

        void Awake()
        {
            _instance = this;
        }

        public List<BlockSurface> GetContactAirSurfaces(BlockCoordinate bc)
        {
            var surfaces = new List<BlockSurface>();

            foreach (BlockSurface surface in System.Enum.GetValues(typeof(BlockSurface)))
            {
                if (IsContactAir(surface, bc))
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

        private bool IsContactAir(BlockSurface surface, BlockCoordinate bc)
        {
            var checkCoordinate = bc.ToVector3() + surface.ToVector3();
            if (BlockCoordinate.IsValid(checkCoordinate))
            {
                if (IsAir(new BlockCoordinate(checkCoordinate)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
