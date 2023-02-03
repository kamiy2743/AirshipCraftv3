using UnityEngine;
using Zenject;
using UnityView.Inputs;

namespace UnityView.Players
{
    internal class DebugPlayerMover : MonoBehaviour
    {
        [Inject] private IInputProvider inputProvider;
        [Inject] private PlayerCamera playerCamera;

        [SerializeField] private float flySpeed;

        private void FixedUpdate()
        {
            transform.position += playerCamera.HorizontalRotation * (inputProvider.DebugFly() * flySpeed * Time.fixedDeltaTime);
        }
    }
}