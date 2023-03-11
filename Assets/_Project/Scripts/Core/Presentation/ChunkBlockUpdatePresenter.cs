using System;
using UniRx;
using UnityView.Rendering.Chunks;
using UseCase;
using Zenject;

namespace Presentation
{
    class ChunkBlockUpdatePresenter : IInitializable, IDisposable
    {
        readonly ChunkBlockSetter chunkBlockSetter;
        // TODO クラス名変更
        readonly BlockUpdateApplier blockUpdateApplier_render;
        readonly UnityView.ChunkCollision.BlockUpdateApplier blockUpdateApplier_collision;

        readonly CompositeDisposable disposals = new();

        internal ChunkBlockUpdatePresenter(ChunkBlockSetter chunkBlockSetter, BlockUpdateApplier blockUpdateApplier_render, UnityView.ChunkCollision.BlockUpdateApplier blockUpdateApplier_collision)
        {
            this.chunkBlockSetter = chunkBlockSetter;
            this.blockUpdateApplier_render = blockUpdateApplier_render;
            this.blockUpdateApplier_collision = blockUpdateApplier_collision;
        }

        public void Initialize()
        {
            chunkBlockSetter
                .OnBlockUpdated
                .Subscribe(coordinate =>
                {
                    blockUpdateApplier_render.Apply(coordinate);
                    blockUpdateApplier_collision.Apply(coordinate);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}