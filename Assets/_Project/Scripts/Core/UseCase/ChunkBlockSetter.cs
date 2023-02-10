using System;
using Domain;
using Domain.Chunks;
using UniRx;

namespace UseCase
{
    public class ChunkBlockSetter
    {
        readonly IChunkRepository _chunkRepository;
        readonly IChunkProvider _chunkProvider;

        readonly Subject<BlockGridCoordinate> _onBlockUpdated = new Subject<BlockGridCoordinate>();
        public IObservable<BlockGridCoordinate> OnBlockUpdated => _onBlockUpdated;

        internal ChunkBlockSetter(IChunkRepository chunkRepository, IChunkProvider chunkProvider)
        {
            _chunkRepository = chunkRepository;
            _chunkProvider = chunkProvider;
        }

        internal void SetBlock(BlockGridCoordinate coordinate, Block block)
        {
            var cgc = ChunkGridCoordinate.Parse(coordinate);
            var rc = RelativeCoordinate.Parse(coordinate);
            var chunk = _chunkProvider.GetChunk(cgc);

            chunk.SetBlock(rc, block);
            _chunkRepository.Store(chunk);

            _onBlockUpdated.OnNext(coordinate);
        }
    }
}