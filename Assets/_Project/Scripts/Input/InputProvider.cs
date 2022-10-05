using UnityEngine;

namespace Input
{
    public static class InputProvider
    {
        private static PlayerInputActions InputActions => _inputActions;
        private static PlayerInputActions _inputActions = SetUp();

        private static PlayerInputActions SetUp()
        {
            var inputActions = new PlayerInputActions();
            inputActions.Enable();
            return inputActions;
        }

        public static Vector3 Move()
        {
            var inputVector = InputActions.Player.HorizontalMove.ReadValue<Vector2>();
            return new Vector3(inputVector.x, 0, inputVector.y).normalized;
        }

        public static Vector3 DebugMove()
        {
            var horizontal = InputActions.Player.HorizontalMove.ReadValue<Vector2>();
            var vertical = InputActions.Player.VerticalMove.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }

        public static bool Jump()
        {
            return InputActions.Player.Jump.triggered;
        }

        public static bool PlaceBlock()
        {
            return InputActions.Player.PlaceBlock.ReadValue<float>() > 0;
        }

        public static bool BreakBlock()
        {
            return InputActions.Player.BreakBlock.ReadValue<float>() > 0;
        }
    }
}
