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
        internal Quaternion VerticalRotation => Quaternion.Euler(cinemachinePOV.m_VerticalAxis.Value, 0, 0);
        internal Vector3 Forward => HorizontalRotation * VerticalRotation * Vector3.forward;

        private void Awake()
        {
            cinemachinePOV = vcam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
        }
    }
}
