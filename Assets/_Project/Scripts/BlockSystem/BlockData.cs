using UnityEngine;
using System.Collections.Generic;
using Util;

namespace BlockSystem
{
    public class BlockData
    {
        public int ID { get; private set; }
        public BlockCoordinate BlockCoordinate { get; private set; }

        public bool IsContactAir { get; private set; }
        private bool[] isContactOtherBlock = new bool[SurfaceNormalExt.EnumCount];

        public BlockData(int id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
        }

        public void SetContactOtherBlockSurfaces(List<SurfaceNormal> surfaces)
        {
            for (int i = 0; i < isContactOtherBlock.Length; i++)
            {
                isContactOtherBlock[i] = false;
            }

            foreach (int surface in surfaces)
            {
                isContactOtherBlock[surface] = true;
            }

            IsContactAir = (surfaces.Count < SurfaceNormalExt.EnumCount);
        }

        public bool IsContactOtherBlock(SurfaceNormal surface)
        {
            return isContactOtherBlock[(int)surface];
        }
    }
}
