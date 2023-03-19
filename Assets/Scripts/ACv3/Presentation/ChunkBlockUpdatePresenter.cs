using System;
using UniRx;
using ACv3.Presentation.Rendering.Chunks;
using ACv3.UseCase;
using Zenject;

namespace ACv3.Presentation
{
    public class ChunkBlockUpdatePresenter : IInitializable, IDisposable
    {
        readonly ChunkBlockSetter chunkBlockSetter;
        // TODO クラス名変更
        readonly BlockUpdateApplier blockUpdateApplier_render;
        readonly Presentation.ChunkCollision.BlockUpdateApplier blockUpdateApplier_collision;

        readonly CompositeDisposable disposals = new();

        internal ChunkBlockUpdatePresenter(ChunkBlockSetter chunkBlockSetter, BlockUpdateApplier blockUpdateApplier_render, Presentation.ChunkCollision.BlockUpdateApplier blockUpdateApplier_collision)
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