using UnityEngine;
using Cinemachine;

namespace UnityView.Players
{
    internal class PlayerCamera
    {
        private CinemachinePOV cinemachinePOV;
        internal Quaternion HorizontalRotation => Quaternion.Euler(0, cinemachinePOV.m_HorizontalAxis.Value, 0);

        internal PlayerCamera(CinemachineVirtualCamera vcam)
        {
            cinemachinePOV = vcam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
        }
    }
}