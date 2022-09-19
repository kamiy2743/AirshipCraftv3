using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のブロックの座標
    /// </summary>
    internal struct BlockCoordinate
    {
        internal readonly int x;
        internal readonly int y;
        internal readonly int z;

        internal BlockCoordinate(Vector3 position) : this((int)position.x, (int)position.y, (int)position.z) { }
        internal BlockCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, z)) throw new System.Exception($"block({x}, {y}, {z}) is invalid");

            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal static bool IsValid(Vector3 position)
        {
            return IsValid((int)position.x, (int)position.y, (int)position.z);
        }
        internal static bool IsValid(int x, int y, int z)
        {
            if (x < 0 || x >= World.WorldBlockSideXZ) return false;
            if (y < 0 || y >= World.WorldBlockSideY) return false;
            if (z < 0 || z >= World.WorldBlockSideXZ) return false;
            return true;
        }

        internal static BlockCoordinate FromChunkAndLocal(ChunkCoordinate cc, LocalCoordinate lc)
        {
            return new BlockCoordinate(
                cc.x * World.ChunkBlockSide + lc.x,
                cc.y * World.ChunkBlockSide + lc.y,
                cc.z * World.ChunkBlockSide + lc.z
            );
        }

        internal Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
