using UnityEngine;
using Zenject;
using UnityView.Inputs;

namespace UnityView.Players
{
    class DebugPlayerMover : MonoBehaviour
    {
        [Inject] IInputProvider _inputProvider;
        [Inject] PlayerCamera _playerCamera;

        [SerializeField] float flySpeed;

        void FixedUpdate()
        {
            transform.position += _playerCamera.HorizontalRotation * _inputProvider.DebugFly() * (flySpeed * Time.fixedDeltaTime);
        }
    }
}