using System;
using ACv3.Domain.Inventories;
using UniRx;
using UnityEngine;

namespace ACv3.UnityView.Inputs
{
    public interface IInputController
    {
        Vector3 DebugFly();
        
        bool PlaceBlock();
        bool BreakBlock();

        public IObservable<ItemBarScrollDirection> OnItemBarScroll();

        public IObservable<Unit> OnOpenPlayerInventoryRequested();
        public IObservable<Unit> OnCloseInventoryRequested();
    }
}