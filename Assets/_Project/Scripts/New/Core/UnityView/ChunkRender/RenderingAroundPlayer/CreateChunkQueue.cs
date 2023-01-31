using System;
using System.Collections.Generic;
using Domain;
using Utils;

namespace UnityView.ChunkRender
{
    internal class CreateChunkQueue
    {
        private PriorityQueue<CreateChunkQueueElement> queue;
        internal int Count => queue.Count;

        internal CreateChunkQueue(int capacity)
        {
            queue = new PriorityQueue<CreateChunkQueueElement>(capacity, new CreateChunkQueueElementComparer());
        }

        internal void Enqueue(int distance, ChunkGridCoordinate chunkGridCoordinate)
        {
            queue.Enqueue(new CreateChunkQueueElement(distance, chunkGridCoordinate));
        }

        internal bool TryDequeue(out ChunkGridCoordinate result)
        {
            try
            {
                result = queue.Dequeue().chunkGridCoordinate;
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private record CreateChunkQueueElement : IComparable<CreateChunkQueueElement>
        {
            internal readonly int distance;
            internal readonly ChunkGridCoordinate chunkGridCoordinate;

            internal CreateChunkQueueElement(int distance, ChunkGridCoordinate chunkGridCoordinate)
            {
                this.distance = distance;
                this.chunkGridCoordinate = chunkGridCoordinate;
            }

            public int CompareTo(CreateChunkQueueElement other) => 0;
        }

        private class CreateChunkQueueElementComparer : IComparer<CreateChunkQueueElement>
        {
            public int Compare(CreateChunkQueueElement a, CreateChunkQueueElement b)
            {
                if (a.distance < b.distance) return 1;
                if (a.distance >= b.distance) return -1;
                return 0;
            }
        }
    }
}