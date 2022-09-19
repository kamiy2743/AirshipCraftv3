using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockSystem;

namespace Player
{
    internal class BlockInteractor : MonoBehaviour
    {
        [SerializeField] private Transform startPosition;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float distance;

        private void Update()
        {
            if (!Physics.Raycast(startPosition.position, playerCamera.Forward, out RaycastHit hit, distance)) return;
            if (!hit.transform.TryGetComponent<ChunkObject>(out var chunkObject)) return;
        }
    }
}
