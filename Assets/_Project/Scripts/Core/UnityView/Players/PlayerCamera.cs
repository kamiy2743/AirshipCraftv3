using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

namespace UnityView.Players
{
    class PlayerCamera : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera playerVcam;

        CinemachinePOV _cinemachinePov;
        Camera _cameraMain;

        internal Quaternion HorizontalRotation => Quaternion.Euler(0, _cinemachinePov.m_HorizontalAxis.Value, 0);
        Quaternion VerticalRotation => Quaternion.Euler(_cinemachinePov.m_VerticalAxis.Value, 0, 0);
        internal Vector3 Forward => HorizontalRotation * VerticalRotation * Vector3.forward;
        internal Vector3 Position => playerVcam.transform.position;

        internal float4x4 ViewportMatrix => math.mul(_cameraMain.projectionMatrix, _cameraMain.worldToCameraMatrix);

        void Awake()
        {
            _cinemachinePov = playerVcam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
            _cameraMain = Camera.main;
        }
    }
}