using System;
using Zenject;
using UniRx;
using UseCase;

namespace Presentation
{
    class ChunkBlockUpdatePresenter : IInitializable, IDisposable
    {
        readonly ChunkBlockSetter _chunkBlockSetter;
        // TODO クラス名変更
        readonly UnityView.Rendering.Chunks.BlockUpdateApplier _blockUpdateApplierRender;
        readonly UnityView.ChunkCollision.BlockUpdateApplier _blockUpdateApplierCollision;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        internal ChunkBlockUpdatePresenter(ChunkBlockSetter chunkBlockSetter, UnityView.Rendering.Chunks.BlockUpdateApplier blockUpdateApplierRender, UnityView.ChunkCollision.BlockUpdateApplier blockUpdateApplierCollision)
        {
            _chunkBlockSetter = chunkBlockSetter;
            _blockUpdateApplierRender = blockUpdateApplierRender;
            _blockUpdateApplierCollision = blockUpdateApplierCollision;
        }

        public void Initialize()
        {
            _chunkBlockSetter
                .OnBlockUpdated
                .Subscribe(coordinate =>
                {
                    _blockUpdateApplierRender.Apply(coordinate);
                    _blockUpdateApplierCollision.Apply(coordinate);
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}