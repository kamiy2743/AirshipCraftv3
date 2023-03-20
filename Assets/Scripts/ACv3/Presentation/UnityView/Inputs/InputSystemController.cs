using System;
using ACv3.Domain.Inventories;
using ACv3.Extensions;
using UniRx;
using UnityEngine;

namespace ACv3.Presentation.Inputs
{
    public class InputSystemController : IInputController
    {
        readonly PlayerInputActions inputActions;

        InputSystemController()
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();
        }

        Vector3 IInputController.DebugFly()
        {
            var horizontal = inputActions.Player.HorizontalMove.ReadValue<Vector2>();
            var vertical = inputActions.Player.VerticalMove.ReadValue<float>();

            return new Vector3(horizontal.x, vertical, horizontal.y).normalized;
        }

        bool IInputController.PlaceBlock()
        {
            return inputActions.Player.PlaceBlock.ReadValue<float>() > 0;
        }

        bool IInputController.BreakBlock()
        {
            return inputActions.Player.BreakBlock.ReadValue<float>() > 0;
        }

        IObservable<ItemBarScrollDirection> IInputController.OnItemBarScroll() => inputActions.Player.ItemBarScroll.AsObservable()
            .Select(context => context.ReadValue<float>())
            .Select(value => value > 0 ? ItemBarScrollDirection.Left : ItemBarScrollDirection.Right)
            .Publish()
            .RefCount();

        IObservable<Unit> IInputController.OnOpenPlayerInventoryRequested() => inputActions.Player.OpenPlayerInventory.TriggeredAsObservable();
        IObservable<Unit> IInputController.OnCloseInventoryRequested() => inputActions.Player.CloseInvenotry.TriggeredAsObservable();
    }
}