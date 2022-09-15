using UnityEngine;
using System.Collections.Generic;
using Util;

namespace BlockSystem
{
    public struct BlockData
    {
        public readonly BlockID ID;
        public readonly BlockCoordinate BlockCoordinate;

        private ContactSurfaces contactOtherBlockSurfaces;
        public bool IsContactAir => !contactOtherBlockSurfaces.IsFull;

        public BlockData(BlockID id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
            contactOtherBlockSurfaces = new ContactSurfaces();
        }

        public void SetContactOtherBlockSurfaces(ContactSurfaces surfaces)
        {
            contactOtherBlockSurfaces = surfaces;
        }

        public bool IsContactOtherBlock(SurfaceNormal surface)
        {
            return contactOtherBlockSurfaces.Contains(surface);
        }
    }
}
