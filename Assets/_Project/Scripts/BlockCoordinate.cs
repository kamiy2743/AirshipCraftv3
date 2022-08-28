using UnityEngine;
using WorldSystem;

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
            if (x < 0 || x >= WorldSettings.WorldBlockSideXZ) throw new System.Exception("x座標が不正です: " + x);
            if (y < 0 || y >= WorldSettings.WorldBlockSideY) throw new System.Exception("y座標が不正です: " + y);
            if (z < 0 || z >= WorldSettings.WorldBlockSideXZ) throw new System.Exception("z座標が不正です: " + z);

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static BlockCoordinate FromChunkAndLocal(ChunkCoordinate cc, LocalCoordinate lc)
        {
            return new BlockCoordinate(
                cc.x * WorldSettings.LocalBlockSide + lc.x,
                cc.y * WorldSettings.LocalBlockSide + lc.y,
                cc.z * WorldSettings.LocalBlockSide + lc.z
            );
        }
    }
}
