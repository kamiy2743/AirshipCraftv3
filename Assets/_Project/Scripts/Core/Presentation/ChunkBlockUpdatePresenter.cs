using System;
using Zenject;
using UniRx;
using UseCase;
using UnityView.Rendering.Chunks;
using UnityView.ChunkCollision;

namespace Presentation
{
    internal class ChunkBlockUpdatePresenter : IInitializable, IDisposable
    {
        private ChunkBlockSetter chunkBlockSetter;
        // TODO クラス名変更
        private UnityView.Rendering.Chunks.BlockUpdateApplier blockUpdateApplier_render;
        private UnityView.ChunkCollision.BlockUpdateApplier blockUpdateApplier_collision;

        private CompositeDisposable disposals = new CompositeDisposable();

        internal ChunkBlockUpdatePresenter(ChunkBlockSetter chunkBlockSetter, UnityView.Rendering.Chunks.BlockUpdateApplier blockUpdateApplier_render, UnityView.ChunkCollision.BlockUpdateApplier blockUpdateApplier_collision)
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