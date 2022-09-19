using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using Cinemachine;

namespace Player
{
    internal class PlayerMover : MonoBehaviour
    {
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private float moveSpeed;

        private void Update()
        {
            transform.position += playerCamera.HorizontalRotation * (InputProvider.DebugMove() * moveSpeed * Time.deltaTime);
        }
    }
}
