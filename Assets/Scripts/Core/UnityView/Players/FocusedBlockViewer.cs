using System;
using UniRx;
using UnityView.Rendering;
using Zenject;

namespace UnityView.Players
{
    class FocusedBlockViewer : IInitializable, IDisposable
    {
        readonly FocusedBlockInfoProvider focusedBlockInfoProvider;
        readonly BlockMeshProvider blockMeshProvider;
        readonly FocusedBlockOutline focusedBlockOutline;

        readonly CompositeDisposable disposals = new();

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

                    var blockMesh = blockMeshProvider.GetBlockMesh(focusedBlockInfo.blockType);
                    focusedBlockOutline.SetMesh(blockMesh.value);
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