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

        public static Vector3 DebugMove()
        {
            var horizontal = InputActions.Player.DebugMoveHorizontal.ReadValue<Vector2>();
            var vertical = InputActions.Player.DebugMoveVertical.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }
    }
}
