using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockSystem;
using MasterData.Block;
using Input;
using UniRx;
using UniRx.Triggers;

namespace Player
{
    internal class BlockInteractor : MonoBehaviour
    {
        [SerializeField] private BlockOutline blockOutline;
        [SerializeField] private Transform startPosition;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float distance;

        private void Start()
        {
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
                .ThrottleFirst(System.TimeSpan.FromSeconds(1))
                .TakeUntil(stopPlaceBlockStream)
                .RepeatUntilDestroy(gameObject)
                .Subscribe(_ => PlaceBlock(raycastHit.normal))
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

        private void PlaceBlock(Vector3 hitNormal)
        {
            Debug.Log("place");
        }
    }
}
