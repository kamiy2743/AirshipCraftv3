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
        
        public static IObservable<T> AsObservable<T>(this InputAction action) where T : struct
        {
            return action.AsObservable()
                .Select(context => context.ReadValue<T>())
                .Publish()
                .RefCount();
        }

        public static IObservable<Unit> TriggeredAsObservable(this InputAction action)
        {
            return action.AsObservable()
                .Select(x => x.ReadValueAsButton())
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }
    }
}