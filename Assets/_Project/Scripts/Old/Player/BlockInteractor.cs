using UnityEngine;
using MasterData.Block;
using Input;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Util;
using DataObject.Block;
using BlockBehaviour.Interface;
using BlockOperator;
using DataStore;

namespace Player
{
    public class BlockInteractor : MonoBehaviour
    {
        [SerializeField] private BlockOutline blockOutline;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float distance;
        [SerializeField] private float placeBlockInterval;
        [SerializeField] private float breakBlockInterval;

        private MasterBlockDataStore _masterBlockDataStore;
        private BlockDataAccessor _blockDataAccessor;
        private PlaceBlockSystem _placeBlockSystem;
        private BreakBlockSystem _breakBlockSystem;
        private CancellationToken _cancellationToken;

        public void StartInitial(MasterBlockDataStore masterBlockDataStore, BlockDataAccessor blockDataAccessor, PlaceBlockSystem placeBlockSystem, BreakBlockSystem breakBlockSystem)
        {
            _masterBlockDataStore = masterBlockDataStore;
            _blockDataAccessor = blockDataAccessor;
            _placeBlockSystem = placeBlockSystem;
            _breakBlockSystem = breakBlockSystem;
            _cancellationToken = this.GetCancellationTokenOnDestroy();

            var selectedBlock = new ReactiveProperty<BlockData>();
            RaycastHit raycastHit = default;
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    selectedBlock.Value = RaycastToBlock(out raycastHit);
                })
                .AddTo(this);

            selectedBlock
                .Subscribe(_ => SetBlockOutline(selectedBlock.Value))
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
                .Subscribe(_ => BreakBlock(selectedBlock.Value))
                .AddTo(this);

            var interactBlockStream = this.UpdateAsObservable().Where(_ => InputProvider.InteractBlock());
            interactBlockStream
                .Where(_ => selectedBlock.Value != BlockData.Empty)
                .Select(_ => _masterBlockDataStore.GetData(selectedBlock.Value.ID).InteractedBehaviour)
                .Where(behaviour => behaviour is not null)
                .Subscribe(behaviour => InteractBlock(selectedBlock.Value, behaviour))
                .AddTo(this);
        }

        private BlockData RaycastToBlock(out RaycastHit raycastHit)
        {
            if (!Physics.Raycast(playerCamera.Position, playerCamera.Forward, out raycastHit, distance, 1 << Layer.Block))
            {
                return BlockData.Empty;
            }

            return _blockDataAccessor.GetBlockData(raycastHit.point - (raycastHit.normal * 0.5f), _cancellationToken);
        }

        private void SetBlockOutline(BlockData selectedBlock)
        {
            if (selectedBlock == BlockData.Empty)
            {
                blockOutline.SetVisible(false);
                return;
            }

            var meshData = _masterBlockDataStore.GetData(selectedBlock.ID).MeshData;
            blockOutline.SetMesh(meshData);
            blockOutline.SetPosition(selectedBlock.BlockCoordinate);
            blockOutline.SetVisible(true);
        }

        private void PlaceBlock(BlockData selectedBlock, Vector3 hitNormal)
        {
            var position = selectedBlock.BlockCoordinate.ToVector3() + hitNormal;
            _placeBlockSystem.PlaceBlock(BlockID.MUCore, position, _cancellationToken);
        }

        private void BreakBlock(BlockData targetBlockData)
        {
            _breakBlockSystem.BreakBlock(targetBlockData, _cancellationToken);
        }

        private void InteractBlock(BlockData targetBlockData, IInteractedBehaviour behaviour)
        {
            behaviour.OnInteracted(targetBlockData);
        }
    }
}
