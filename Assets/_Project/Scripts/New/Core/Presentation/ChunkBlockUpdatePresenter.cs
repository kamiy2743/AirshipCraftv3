using System;
using UseCase;
using Zenject;
using UniRx;

namespace Presentation
{
    internal class ChunkBlockUpdatePresenter : IInitializable, IDisposable
    {
        private ChunkBlockSetter chunkBlockSetter;

        private CompositeDisposable disposals = new CompositeDisposable();

        internal ChunkBlockUpdatePresenter(ChunkBlockSetter chunkBlockSetter)
        {
            this.chunkBlockSetter = chunkBlockSetter;
        }

        public void Initialize()
        {
            chunkBlockSetter
                .OnBlockUpdated
                .Subscribe(coordinate =>
                {
                    UnityEngine.Debug.Log("updated: " + coordinate);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}