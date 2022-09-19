using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockSystem;
using MasterData.Block;

namespace Player
{
    internal class BlockInteractor : MonoBehaviour
    {
        [SerializeField] private BlockOutline blockOutline;
        [SerializeField] private Transform startPosition;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float distance;

        private void Update()
        {
            blockOutline.SetVisible(false);

            if (!Physics.Raycast(startPosition.position, playerCamera.Forward, out RaycastHit hit, distance)) return;
            if (!hit.transform.TryGetComponent<IBlockDataAccessor>(out var blockDataAccessor)) return;

            var blockData = blockDataAccessor.GetBlockData(hit.point - hit.normal * 0.5f);
            var meshData = MasterBlockDataStore.GetData(blockData.ID).MeshData;

            blockOutline.SetMesh(meshData);
            blockOutline.SetPosition(blockData.BlockCoordinate);
            blockOutline.SetVisible(true);
        }
    }
}
