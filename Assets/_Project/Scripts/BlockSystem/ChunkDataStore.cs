using System.Threading;
using System.Collections.Generic;
using UniRx;
using DataObject.Chunk;

namespace BlockSystem
{
    /// <summary> チャンクデータを管理 </summary>
    internal class ChunkDataStore
    {
        private static readonly object SyncObject = new object();

        private ChunkDataFileIO _chunkDataFileIO;

        private const int CacheCapacity = 256;
        private Dictionary<ChunkCoordinate, ChunkData> chunkDataCache = new Dictionary<ChunkCoordinate, ChunkData>(CacheCapacity);

        private HashSet<ChunkData> reusableChunkHashSet = new HashSet<ChunkData>();
        private Queue<ChunkData> reusableChunkQueue = new Queue<ChunkData>();


        internal ChunkDataStore(ChunkDataFileIO chunkDataFileIO)
        {
            _chunkDataFileIO = chunkDataFileIO;
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// キャンセルされた場合はnullを返す
        /// 使い終わったら参照を開放することを忘れないように
        /// </summary>
        internal ChunkData GetChunkData(ChunkCoordinate cc, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return null;

            lock (SyncObject)
            {
                // キャッシュにあればそれを返す
                if (chunkDataCache.ContainsKey(cc))
                {
                    var cache = (ChunkData)chunkDataCache[cc];

                    // 再利用リストにあれば削除
                    TryRemoveReusableChunk(cache);
                    cache.ReferenceCounter.AddRef();

                    return cache;
                }

                // 再利用可能チャンクの取得
                var useReusableChunkData = TryGetReusableChunkData(out ChunkData reusableChunkData);

                ChunkData chunkData;
                // 保存されていれば読み込み、無ければ作成
                if (!_chunkDataFileIO.Read(cc, reusableChunkData, out chunkData))
                {
                    chunkData = CreateNewChunk(cc, reusableChunkData);
                }

                // キャッシュに追加
                chunkDataCache.Add(cc, chunkData);

                if (ct.IsCancellationRequested) return null;

                // 参照追加
                chunkData.ReferenceCounter.AddRef();

                // 新規作成時のみ購読
                if (!useReusableChunkData)
                {
                    // 参照がなくなったら再利用リストに追加
                    chunkData.ReferenceCounter.OnAllReferenceReleased.Subscribe(_ => AddReusableChunk(chunkData));
                }

                return chunkData;
            }
        }

        /// <summary> 再利用可能なチャンクの取得 </summary>
        private bool TryGetReusableChunkData(out ChunkData reusableChunkData)
        {
            reusableChunkData = null;

            // キャッシュが埋まってなければ再利用はしない
            if (chunkDataCache.Count < CacheCapacity)
            {
                return false;
            }

            while (reusableChunkQueue.TryDequeue(out var takeChunk))
            {
                // キューには再利用可能チャンクから除外されているものも含まれているので、利用可能かチェックする
                if (!reusableChunkHashSet.Contains(takeChunk)) continue;

                reusableChunkData = takeChunk;
                reusableChunkHashSet.Remove(takeChunk);
                chunkDataCache.Remove(takeChunk.ChunkCoordinate);
                return true;
            }

            return false;
        }

        /// <summary> 再利用可能チャンクに追加 </summary>
        private void AddReusableChunk(ChunkData addChunk)
        {
            // イベントで呼び出されることを想定して自前でlockする
            lock (SyncObject)
            {
                reusableChunkHashSet.Add(addChunk);
                reusableChunkQueue.Enqueue(addChunk);
            }
        }

        /// <summary> 再利用可能チャンクから削除 </summary>
        private bool TryRemoveReusableChunk(ChunkData removeChunk)
        {
            // キューはランダムアクセスができないのでここでは削除しない
            return reusableChunkHashSet.Remove(removeChunk);
        }

        /// <summary> チャンクを新規作成し保存 </summary>
        private ChunkData CreateNewChunk(ChunkCoordinate cc, ChunkData reusableChunkData)
        {
            ChunkData newChunkData;

            // 再利用可能なChunkDataがあれば再利用する
            if (reusableChunkData is not null)
            {
                newChunkData = reusableChunkData.ReuseConstructor(cc);
            }
            else
            {
                newChunkData = ChunkData.NewConstructor(cc);
            }

            // 保存
            _chunkDataFileIO.Append(newChunkData);

            return newChunkData;
        }
    }
}
