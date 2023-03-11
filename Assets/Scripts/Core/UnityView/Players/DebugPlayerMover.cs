using UnityEngine;
using UnityView.Inputs;
using Zenject;

namespace UnityView.Players
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