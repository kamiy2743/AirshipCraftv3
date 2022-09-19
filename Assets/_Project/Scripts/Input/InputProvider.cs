using System.Collections;
using System.Collections.Generic;
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
            var inputVector = InputActions.Player.Move.ReadValue<Vector2>();
            return new Vector3(inputVector.x, 0, inputVector.y).normalized;
        }

        public static Vector3 DebugMove()
        {
            var horizontal = InputActions.Player.DebugMoveHorizontal.ReadValue<Vector2>();
            var vertical = InputActions.Player.DebugMoveVertical.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }

        public static bool Jump()
        {
            return InputActions.Player.Jump.triggered;
        }
    }
}
