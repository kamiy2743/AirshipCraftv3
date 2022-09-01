using UnityEngine;
using System.Collections.Generic;
using Util;

namespace BlockSystem
{
    public class BlockData
    {
        public int ID { get; private set; }
        public BlockCoordinate BlockCoordinate { get; private set; }

        public bool IsContactAir => _contactAirSurfaces.Count > 0;
        public IReadOnlyList<SurfaceNormal> ContactAirSurfaces => _contactAirSurfaces;
        private List<SurfaceNormal> _contactAirSurfaces = new List<SurfaceNormal>();

        public BlockData(int id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
        }

        public void SetContactSurfaces(List<SurfaceNormal> contactAirSurfaces)
        {
            _contactAirSurfaces = contactAirSurfaces;
        }
    }
}
