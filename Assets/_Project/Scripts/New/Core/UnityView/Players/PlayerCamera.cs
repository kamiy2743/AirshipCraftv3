using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

namespace UnityView.Players
{
    internal class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera playerVcam;

        private CinemachinePOV cinemachinePOV;
        private Camera cameraMain;

        internal Quaternion HorizontalRotation => Quaternion.Euler(0, cinemachinePOV.m_HorizontalAxis.Value, 0);
        internal Quaternion VerticalRotation => Quaternion.Euler(cinemachinePOV.m_VerticalAxis.Value, 0, 0);
        internal Vector3 Forward => HorizontalRotation * VerticalRotation * Vector3.forward;

        internal float4x4 ViewportMatrix => math.mul(cameraMain.projectionMatrix, cameraMain.worldToCameraMatrix);

        private void Awake()
        {
            cinemachinePOV = playerVcam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
            cameraMain = Camera.main;
        }
    }
}