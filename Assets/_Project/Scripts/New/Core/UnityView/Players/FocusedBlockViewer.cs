using System;
using Zenject;
using UniRx;
using UnityView.Rendering;

namespace UnityView.Players
{
    internal class FocusedBlockViewer : IInitializable, IDisposable
    {
        private FocusedBlockInfoProvider focusedBlockInfoProvider;
        private BlockMeshProvider blockMeshProvider;
        private FocusedBlockOutline focusedBlockOutline;

        private CompositeDisposable disposals = new CompositeDisposable();

        internal FocusedBlockViewer(FocusedBlockInfoProvider focusedBlockInfoProvider, BlockMeshProvider blockMeshProvider, FocusedBlockOutline focusedBlockOutline)
        {
            this.focusedBlockInfoProvider = focusedBlockInfoProvider;
            this.blockMeshProvider = blockMeshProvider;
            this.focusedBlockOutline = focusedBlockOutline;
        }

        public void Initialize()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    if (!focusedBlockInfoProvider.TryGetFocusedBlockInfo(out var focusedBlockInfo))
                    {
                        focusedBlockOutline.SetVisible(false);
                        return;
                    }

                    var blockMesh = blockMeshProvider.GetBlockMesh(focusedBlockInfo.blockTypeID);
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