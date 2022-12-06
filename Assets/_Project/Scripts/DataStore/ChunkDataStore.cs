using System;
using System.Threading;
using System.Collections.Generic;
using UniRx;
using DataObject.Chunk;

namespace DataStore
{
    /// <summary> チャンクデータを管理 </summary>
    public class ChunkDataStore : IDisposable
    {
        private ChunkDataFileIO _chunkDataFileIO;

        private const int CacheCapacity = 256;
        private Dictionary<ChunkCoordinate, ChunkData> allocatedChunkData = new Dictionary<ChunkCoordinate, ChunkData>(CacheCapacity);

        private Queue<ChunkData> reusableChunkQueue = new Queue<ChunkData>();

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public ChunkDataStore(ChunkDataFileIO chunkDataFileIO)
        {
            _chunkDataFileIO = chunkDataFileIO;
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// キャンセルされた場合はnullを返す
        /// 使い終わったら参照を開放することを忘れないように
        /// </summary>
        public ChunkData GetChunkData(ChunkCoordinate cc, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return null;

            rwLock.EnterReadLock();
            try
            {
                // キャッシュにあればそれを返す
                if (allocatedChunkData.TryGetValue(cc, out var cache))
                {
                    cache.ReferenceCounter.AddRef();
                    return cache;
                }
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            rwLock.EnterWriteLock();
            try
            {
                // 再利用可能チャンクの取得
                var useReusableChunkData = TryGetReusableChunkData(out ChunkData reusableChunkData);

                ChunkData chunkData;
                // 保存されていれば読み込み、無ければ作成
                if (!_chunkDataFileIO.Read(cc, reusableChunkData, out chunkData))
                {
                    chunkData = CreateNewChunk(cc, reusableChunkData);
                }

                // キャッシュに追加
                allocatedChunkData.Add(cc, chunkData);

                if (ct.IsCancellationRequested) return null;

                // 参照追加
                chunkData.ReferenceCounter.AddRef();

                // 再利用しなかった=新規作成 時のみ購読
                if (!useReusableChunkData)
                {
                    // 参照がなくなったら再利用リストに追加
                    chunkData.ReferenceCounter.OnAllReferenceReleased.Subscribe(_ =>
                    {
                        rwLock.EnterWriteLock();
                        reusableChunkQueue.Enqueue(chunkData);
                        rwLock.ExitWriteLock();
                    });
                }

                return chunkData;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        /// <summary> 再利用可能なチャンクの取得 </summary>
        private bool TryGetReusableChunkData(out ChunkData reusableChunkData)
        {
            reusableChunkData = null;

            // キャッシュが埋まってなければ再利用はしない
            if (allocatedChunkData.Count < CacheCapacity)
            {
                return false;
            }

            while (reusableChunkQueue.TryDequeue(out var takeChunk))
            {
                // キューには再利用可能チャンクから除外されているものも含まれているので、利用可能かチェックする
                if (!takeChunk.ReferenceCounter.IsFree()) continue;

                reusableChunkData = takeChunk;
                allocatedChunkData.Remove(takeChunk.ChunkCoordinate);
                return true;
            }

            return false;
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

        public void Dispose()
        {
            rwLock.Dispose();
            foreach (var chunkData in allocatedChunkData.Values)
            {
                chunkData.Dispose();
            }
        }
    }
}
