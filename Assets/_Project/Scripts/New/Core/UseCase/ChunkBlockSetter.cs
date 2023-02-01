using System;
using Domain;
using Domain.Chunks;
using UniRx;

namespace UseCase
{
    public class ChunkBlockSetter
    {
        private IChunkRepository chunkRepository;
        private IChunkProvider chunkProvider;

        private Subject<BlockGridCoordinate> _onBlockUpdated = new Subject<BlockGridCoordinate>();
        public IObservable<BlockGridCoordinate> OnBlockUpdated => _onBlockUpdated;

        internal ChunkBlockSetter(IChunkRepository chunkRepository, IChunkProvider chunkProvider)
        {
            this.chunkRepository = chunkRepository;
            this.chunkProvider = chunkProvider;
        }

        internal void SetBlock(BlockGridCoordinate coordinate, Block block)
        {
            var cgc = ChunkGridCoordinate.Parse(coordinate);
            var rc = RelativeCoordinate.Parse(coordinate);
            var chunk = chunkProvider.GetChunk(cgc);

            chunk.SetBlock(rc, block);
            chunkRepository.Store(chunk);

            _onBlockUpdated.OnNext(coordinate);
        }
    }
}