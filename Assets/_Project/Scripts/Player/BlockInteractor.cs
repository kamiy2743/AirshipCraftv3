using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockSystem;
using MasterData.Block;
using Input;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Player
{
    internal class BlockInteractor : MonoBehaviour
    {
        [SerializeField] private BlockOutline blockOutline;
        [SerializeField] private Transform startPosition;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float distance;
        [SerializeField] private float placeBlockInterval;

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
                .ThrottleFirst(System.TimeSpan.FromSeconds(placeBlockInterval))
                .TakeUntil(stopPlaceBlockStream)
                .RepeatUntilDestroy(gameObject)
                .Subscribe(_ => PlaceBlock(selectedBlock.Value, raycastHit.normal))
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

        private void BreakBlock(BlockData targetBlockData)
        {

        }
    }
}
