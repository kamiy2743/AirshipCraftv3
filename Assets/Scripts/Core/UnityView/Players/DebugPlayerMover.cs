using UnityEngine;
using ACv3.UnityView.Inputs;
using Zenject;

namespace ACv3.UnityView.Players
{
    class DebugPlayerMover : MonoBehaviour
    {
        [Inject] IInputProvider inputProvider;
        [Inject] PlayerCamera playerCamera;

        [SerializeField] float flySpeed;

        void FixedUpdate()
        {
            transform.position += playerCamera.HorizontalRotation * inputProvider.DebugFly() * (flySpeed * Time.fixedDeltaTime);
        }
    }
}