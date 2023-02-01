using System;
using Zenject;
using UniRx;
using UseCase;
using UnityView.ChunkRender;

namespace Presentation
{
    internal class ChunkBlockUpdatePresenter : IInitializable, IDisposable
    {
        private ChunkBlockSetter chunkBlockSetter;
        private BlockUpdateApplier blockUpdateApplier;

        private CompositeDisposable disposals = new CompositeDisposable();

        internal ChunkBlockUpdatePresenter(ChunkBlockSetter chunkBlockSetter, BlockUpdateApplier blockUpdateApplier)
        {
            this.chunkBlockSetter = chunkBlockSetter;
            this.blockUpdateApplier = blockUpdateApplier;
        }

        public void Initialize()
        {
            chunkBlockSetter
                .OnBlockUpdated
                .Subscribe(coordinate =>
                {
                    blockUpdateApplier.Apply(coordinate);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}