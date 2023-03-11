using System;
using UniRx;
using UnityEngine.InputSystem;

namespace ACv3.Extensions
{
    public static class InputSystemToObservableExt
    {
        public static IObservable<InputAction.CallbackContext> AsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => action.performed += h,
                h => action.performed -= h);
        }
    }
}