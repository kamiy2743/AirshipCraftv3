using UnityEngine;

namespace UnityView.Inputs
{
    class InputSystemInputProvider : IInputProvider
    {
        readonly PlayerInputActions _inputActions;

        internal InputSystemInputProvider()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
        }

        public Vector3 DebugFly()
        {
            var horizontal = _inputActions.Player.HorizontalMove.ReadValue<Vector2>();
            var vertical = _inputActions.Player.VerticalMove.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }

        public bool PlaceBlock()
        {
            return _inputActions.Player.PlaceBlock.ReadValue<float>() > 0;
        }

        public bool BreakBlock()
        {
            return _inputActions.Player.BreakBlock.ReadValue<float>() > 0;
        }
    }
}
