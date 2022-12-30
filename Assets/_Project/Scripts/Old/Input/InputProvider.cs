using UnityEngine;

namespace Input
{
    public static class InputProvider
    {
        private static PlayerInputActions inputActions = Setup();

        private static PlayerInputActions Setup()
        {
            var inputActions = new PlayerInputActions();
            inputActions.Enable();
            return inputActions;
        }

        public static Vector3 Move()
        {
            var inputVector = inputActions.Player.HorizontalMove.ReadValue<Vector2>();
            return new Vector3(inputVector.x, 0, inputVector.y).normalized;
        }

        public static Vector3 DebugMove()
        {
            var horizontal = inputActions.Player.HorizontalMove.ReadValue<Vector2>();
            var vertical = inputActions.Player.VerticalMove.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }

        public static bool Jump()
        {
            return inputActions.Player.Jump.triggered;
        }

        public static bool PlaceBlock()
        {
            return inputActions.Player.PlaceBlock.ReadValue<float>() > 0;
        }

        public static bool BreakBlock()
        {
            return inputActions.Player.BreakBlock.ReadValue<float>() > 0;
        }

        public static bool InteractBlock()
        {
            return inputActions.Player.InteractBlock.triggered;
        }
    }
}
