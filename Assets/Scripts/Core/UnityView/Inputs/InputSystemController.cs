using System;
using ACv3.Domain.Inventories;
using ACv3.Extensions;
using UniRx;
using UnityEngine;

namespace ACv3.UnityView.Inputs
{
    public class InputSystemController : IInputController
    {
        readonly PlayerInputActions inputActions;

        internal InputSystemController()
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

        public IObservable<ItemBarScrollDirection> OnItemBarScroll => inputActions.Player.ItemBarScroll.AsObservable()
            .Select(context => context.ReadValue<float>())
            .Select(value => value > 0 ? ItemBarScrollDirection.Left : ItemBarScrollDirection.Right);
    }
}
