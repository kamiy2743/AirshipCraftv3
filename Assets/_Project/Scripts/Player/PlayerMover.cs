using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using Cinemachine;

namespace Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        [SerializeField] private float moveSpeed;

        private CinemachinePOV cinemachinePOV;

        void Start()
        {
            var playerInput = new PlayerInputActions();
            playerInput.Enable();

            cinemachinePOV = playerCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
        }

        void Update()
        {
            var horizontalRotation = Quaternion.Euler(0, cinemachinePOV.m_HorizontalAxis.Value, 0);
            transform.position += horizontalRotation * (InputProvider.DebugMove() * moveSpeed * Time.deltaTime);
        }
    }
}
