using UnityEngine;
using System.Collections.Generic;

namespace BlockSystem
{
    public class BlockData
    {
        public int ID { get; private set; }
        public BlockCoordinate BlockCoordinate { get; private set; }

        public bool IsContactAir => _contactAirSurfaces.Count > 0;
        public IReadOnlyList<BlockSurface> ContactAirSurfaces => _contactAirSurfaces;
        private List<BlockSurface> _contactAirSurfaces = new List<BlockSurface>();

        public BlockData(int id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
        }

        public void SetContactSurfaces(List<BlockSurface> contactAirSurfaces)
        {
            _contactAirSurfaces = contactAirSurfaces;
        }
    }
}
