using UnityEngine;
using BlockSystem;
using MasterData.Block;
using Input;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Player
{
    internal class BlockInteractor : MonoBehaviour
    {
        [SerializeField] private BlockOutline blockOutline;
        [SerializeField] private Transform startPosition;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float distance;
        [SerializeField] private float placeBlockInterval;
        [SerializeField] private float breakBlockInterval;

        private CancellationToken _cancellationToken;

        private void Start()
        {
            _cancellationToken = this.GetCancellationTokenOnDestroy();

            var selectedBlock = new ReactiveProperty<BlockData>();
            selectedBlock
                .Subscribe(_ => SetBlockOutline(selectedBlock.Value))
                .AddTo(this);

            RaycastHit raycastHit = default;
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    selectedBlock.Value = RaycastToBlock(out raycastHit);
                })
                .AddTo(this);

            var placeBlockStream = this.UpdateAsObservable().Where(_ => InputProvider.PlaceBlock());
            var stopPlaceBlockStream = this.UpdateAsObservable().Where(_ => !InputProvider.PlaceBlock());
            placeBlockStream
                .Where(_ => selectedBlock.Value != BlockData.Empty)
                .ThrottleFirst(TimeSpan.FromSeconds(placeBlockInterval))
                .TakeUntil(stopPlaceBlockStream)
                .RepeatUntilDestroy(this)
                .Subscribe(_ => PlaceBlock(selectedBlock.Value, raycastHit.normal))
                .AddTo(this);

            var breakBlockStream = this.UpdateAsObservable().Where(_ => InputProvider.BreakBlock());
            var stopBreakBlockStream = this.UpdateAsObservable().Where(_ => !InputProvider.BreakBlock());
            breakBlockStream
                .Where(_ => selectedBlock.Value != BlockData.Empty)
                .Subscribe(_ => StartBreakBlock(selectedBlock.Value))
                .AddTo(this);
        }

        private BlockData RaycastToBlock(out RaycastHit raycastHit)
        {
            if (!Physics.Raycast(startPosition.position, playerCamera.Forward, out raycastHit, distance))
            {
                return BlockData.Empty;
            }
            if (!raycastHit.transform.TryGetComponent<IBlockDataAccessor>(out var blockDataAccessor))
            {
                return BlockData.Empty;
            }

            return blockDataAccessor.GetBlockData(raycastHit.point - raycastHit.normal * 0.5f);
        }

        private void SetBlockOutline(BlockData selectedBlock)
        {
            if (selectedBlock == BlockData.Empty)
            {
                blockOutline.SetVisible(false);
                return;
            }

            var meshData = MasterBlockDataStore.GetData(selectedBlock.ID).MeshData;
            blockOutline.SetMesh(meshData);
            blockOutline.SetPosition(selectedBlock.BlockCoordinate);
            blockOutline.SetVisible(true);
        }

        private void PlaceBlock(BlockData selectedBlock, Vector3 hitNormal)
        {
            var position = selectedBlock.BlockCoordinate.ToVector3() + (Vector3.one * 0.5f) + hitNormal;
            PlaceBlockSystem.Instance.PlaceBlock(BlockID.Dirt, position, _cancellationToken).Forget();
        }

        private void StartBreakBlock(BlockData targetBlockData)
        {
            BreakBlockSystem.Instance.BreakBlock(targetBlockData.BlockCoordinate, _cancellationToken).Forget();
        }
    }
}
