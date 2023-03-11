using UnityEngine;
using ACv3.UnityView.Inputs;
using Zenject;

namespace ACv3.UnityView.Players
{
    class DebugPlayerMover : MonoBehaviour
    {
        [Inject] IInputController inputController;
        [Inject] PlayerCamera playerCamera;

        [SerializeField] float flySpeed;

        void FixedUpdate()
        {
            transform.position += playerCamera.HorizontalRotation * inputController.DebugFly() * (flySpeed * Time.fixedDeltaTime);
        }
    }
}