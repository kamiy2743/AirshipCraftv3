using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のブロックの座標
    /// </summary>
    public class BlockCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public BlockCoordinate(Vector3 position) : this((int)position.x, (int)position.y, (int)position.z) { }
        public BlockCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, z)) throw new System.Exception($"block({x}, {y}, {z}) is invalid");

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static bool IsValid(Vector3 position)
        {
            return IsValid((int)position.x, (int)position.y, (int)position.z);
        }
        public static bool IsValid(int x, int y, int z)
        {
            if (x < 0 || x >= WorldSettings.WorldBlockSideXZ) return false;
            if (y < 0 || y >= WorldSettings.WorldBlockSideY) return false;
            if (z < 0 || z >= WorldSettings.WorldBlockSideXZ) return false;
            return true;
        }

        public static BlockCoordinate FromChunkAndLocal(ChunkCoordinate cc, LocalCoordinate lc)
        {
            return new BlockCoordinate(
                cc.x * WorldSettings.LocalBlockSide + lc.x,
                cc.y * WorldSettings.LocalBlockSide + lc.y,
                cc.z * WorldSettings.LocalBlockSide + lc.z
            );
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
