using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Player
{
    internal class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vcam;

        private CinemachinePOV cinemachinePOV;

        internal Quaternion HorizontalRotation => Quaternion.Euler(0, cinemachinePOV.m_HorizontalAxis.Value, 0);

        void Awake()
        {
            cinemachinePOV = vcam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
        }
    }
}
