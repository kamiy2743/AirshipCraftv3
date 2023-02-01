using UnityEngine;

namespace UnityView.Inputs
{
    internal class InputSystemInputProvider : IInputProvider
    {
        private PlayerInputActions inputActions;

        internal InputSystemInputProvider()
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();
        }

        public Vector3 DebugFly()
        {
            var horizontal = inputActions.Player.HorizontalMove.ReadValue<Vector2>();
            var vertical = inputActions.Player.VerticalMove.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }

        public bool PlaceBlock()
        {
            return inputActions.Player.PlaceBlock.ReadValue<float>() > 0;
        }

        public bool BreakBlock()
        {
            return inputActions.Player.BreakBlock.ReadValue<float>() > 0;
        }
    }
}
