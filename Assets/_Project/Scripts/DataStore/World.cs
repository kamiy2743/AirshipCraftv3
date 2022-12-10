using UnityEngine;

namespace DataStore
{
    public class World
    {
        /// <summary>
        /// プレイヤーの周りの読み込み距離(チャンク換算)
        /// </summary>
        public static readonly int LoadChunkRadius = 16;
        public static readonly int LoadChunkCount = (int)Mathf.Pow(LoadChunkRadius * 2 + 1, 3);

        /// <summary> 
        /// コライダー等の有効半径 
        /// </summary>
        public static readonly int SimulationRadius = 2;
    }
}
