using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

namespace ACv3.UnityView.Players
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera playerVcam;

        CinemachinePOV cinemachinePOV;
        Camera cameraMain;

        internal Quaternion HorizontalRotation => Quaternion.Euler(0, cinemachinePOV.m_HorizontalAxis.Value, 0);
        internal Quaternion VerticalRotation => Quaternion.Euler(cinemachinePOV.m_VerticalAxis.Value, 0, 0);
        internal Vector3 Forward => HorizontalRotation * VerticalRotation * Vector3.forward;
        internal Vector3 Position => playerVcam.transform.position;

        internal float4x4 ViewportMatrix => math.mul(cameraMain.projectionMatrix, cameraMain.worldToCameraMatrix);

        void Awake()
        {
            cinemachinePOV = playerVcam.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
            cameraMain = Camera.main;
        }
    }
}