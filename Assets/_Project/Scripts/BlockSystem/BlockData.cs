using UnityEngine;
using System.Collections.Generic;
using Util;

namespace BlockSystem
{
    public struct BlockData
    {
        public int ID { get; private set; }
        public BlockCoordinate BlockCoordinate { get; private set; }

        private ContactSurfaces contactOtherBlockSurfaces;
        public bool IsContactAir => !contactOtherBlockSurfaces.IsFull;

        public BlockData(int id, BlockCoordinate bc)
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
