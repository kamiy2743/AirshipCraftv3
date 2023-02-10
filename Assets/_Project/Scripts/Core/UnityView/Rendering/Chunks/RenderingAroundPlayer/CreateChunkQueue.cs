using System;
using System.Collections.Generic;
using Domain;
using Utils;

namespace UnityView.Rendering.Chunks
{
    class CreateChunkQueue
    {
        readonly PriorityQueue<CreateChunkQueueElement> _queue;
        internal int Count => _queue.Count;

        internal CreateChunkQueue(int capacity)
        {
            _queue = new PriorityQueue<CreateChunkQueueElement>(capacity, new CreateChunkQueueElementComparer());
        }

        internal void Enqueue(int distance, ChunkGridCoordinate chunkGridCoordinate)
        {
            _queue.Enqueue(new CreateChunkQueueElement(distance, chunkGridCoordinate));
        }

        internal bool TryDequeue(out ChunkGridCoordinate result)
        {
            try
            {
                result = _queue.Dequeue().ChunkGridCoordinate;
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        record CreateChunkQueueElement : IComparable<CreateChunkQueueElement>
        {
            internal readonly int Distance;
            internal readonly ChunkGridCoordinate ChunkGridCoordinate;

            internal CreateChunkQueueElement(int distance, ChunkGridCoordinate chunkGridCoordinate)
            {
                Distance = distance;
                ChunkGridCoordinate = chunkGridCoordinate;
            }

            public int CompareTo(CreateChunkQueueElement other) => 0;
        }

        class CreateChunkQueueElementComparer : IComparer<CreateChunkQueueElement>
        {
            public int Compare(CreateChunkQueueElement a, CreateChunkQueueElement b)
            {
                if (a.Distance < b.Distance) return 1;
                if (a.Distance >= b.Distance) return -1;
                return 0;
            }
        }
    }
}