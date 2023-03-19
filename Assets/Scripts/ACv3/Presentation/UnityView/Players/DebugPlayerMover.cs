using UnityEngine;
using ACv3.Presentation.Inputs;
using Zenject;

namespace ACv3.Presentation.Players
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