using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のチャンクの座標
    /// 座標というよりはインデックスに近い
    /// </summary>
    public class ChunkCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public ChunkCoordinate(int x, int y, int z)
        {
            if (x < 0 || x >= WorldSettings.WorldChunkSideXZ) throw new System.Exception("チャンクx座標が不正です: " + x);
            if (y < 0 || y >= WorldSettings.WorldChunkSideY) throw new System.Exception("チャンクy座標が不正です: " + y);
            if (z < 0 || z >= WorldSettings.WorldChunkSideXZ) throw new System.Exception("チャンクz座標が不正です: " + z);

            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
