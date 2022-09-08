using UnityEngine;
using System.Collections.Generic;
using Util;

namespace BlockSystem
{
    public class BlockData
    {
        public int ID { get; private set; }
        public BlockCoordinate BlockCoordinate { get; private set; }

        private ContactSurfaces contactOtherBlockSurfaces = new ContactSurfaces();
        public bool IsContactAir => !contactOtherBlockSurfaces.IsFull;

        public BlockData(int id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
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
