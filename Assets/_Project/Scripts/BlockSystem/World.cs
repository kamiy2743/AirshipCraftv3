using UnityEngine;

namespace BlockSystem
{
    internal class World
    {
        /// <summary>プレイヤーの周りの読み込み距離(チャンク換算)</summary>
        internal static readonly int LoadChunkRadius = 0;
        internal static readonly int LoadChunkCount = (int)Mathf.Pow(LoadChunkRadius * 2 + 1, 3);
    }
}
