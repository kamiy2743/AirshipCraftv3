using System;
using ACv3.Domain;
using ACv3.Domain.Chunks;
using UniRx;

namespace ACv3.UseCase
{
    public class ChunkBlockSetter
    {
        readonly IChunkRepository chunkRepository;
        readonly IChunkProvider chunkProvider;

        readonly Subject<BlockGridCoordinate> _onBlockUpdated = new();
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