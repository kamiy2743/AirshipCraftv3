using System;
using Zenject;
using UniRx;
using UnityView.Rendering;

namespace UnityView.Players
{
    class FocusedBlockViewer : IInitializable, IDisposable
    {
        readonly FocusedBlockInfoProvider _focusedBlockInfoProvider;
        readonly BlockMeshProvider _blockMeshProvider;
        readonly FocusedBlockOutline _focusedBlockOutline;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        internal FocusedBlockViewer(FocusedBlockInfoProvider focusedBlockInfoProvider, BlockMeshProvider blockMeshProvider, FocusedBlockOutline focusedBlockOutline)
        {
            _focusedBlockInfoProvider = focusedBlockInfoProvider;
            _blockMeshProvider = blockMeshProvider;
            _focusedBlockOutline = focusedBlockOutline;
        }

        public void Initialize()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    if (!_focusedBlockInfoProvider.TryGetFocusedBlockInfo(out var focusedBlockInfo))
                    {
                        _focusedBlockOutline.SetVisible(false);
                        return;
                    }

                    var blockMesh = _blockMeshProvider.GetBlockMesh(focusedBlockInfo.BlockType);
                    _focusedBlockOutline.SetMesh(blockMesh.Value);
                    _focusedBlockOutline.SetVisible(true);
                    _focusedBlockOutline.SetPivot(focusedBlockInfo.PivotCoordinate);
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}