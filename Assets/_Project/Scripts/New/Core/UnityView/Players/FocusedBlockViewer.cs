using System;
using Zenject;
using UniRx;
using UnityView.ChunkRender;

namespace UnityView.Players
{
    internal class FocusedBlockViewer : IInitializable, IDisposable
    {
        private FocusedBlockProvider focusedBlockProvider;
        private BlockMeshProvider blockMeshProvider;
        private FocusedBlockOutline focusedBlockOutline;

        private CompositeDisposable disposals = new CompositeDisposable();

        internal FocusedBlockViewer(FocusedBlockProvider focusedBlockProvider, BlockMeshProvider blockMeshProvider, FocusedBlockOutline focusedBlockOutline)
        {
            this.focusedBlockProvider = focusedBlockProvider;
            this.blockMeshProvider = blockMeshProvider;
            this.focusedBlockOutline = focusedBlockOutline;
        }

        public void Initialize()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    if (!focusedBlockProvider.TryGetFocusedBlockInfo(out var focusedBlockInfo))
                    {
                        focusedBlockOutline.SetVisible(false);
                        return;
                    }

                    var blockMesh = blockMeshProvider.GetBlockMesh(focusedBlockInfo.block.blockTypeID);
                    focusedBlockOutline.SetMesh(blockMesh.All);
                    focusedBlockOutline.SetVisible(true);
                    focusedBlockOutline.SetPivot(focusedBlockInfo.pivotCoordinate);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}