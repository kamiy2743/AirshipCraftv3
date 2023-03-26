using System;
using UniRx;
using UniRx.Triggers;

namespace ACv3.UI.View
{
    public class InteractableSlotView : SlotView
    {
        public IObservable<Unit> OnCursored => GetComponent<ObservablePointerEnterTrigger>().OnPointerEnterAsObservable().AsUnitObservable();
        public IObservable<Unit> OnUnCursored => GetComponent<ObservablePointerExitTrigger>().OnPointerExitAsObservable().AsUnitObservable();
        public IObservable<Unit> OnClocked => GetComponent<ObservablePointerClickTrigger>().OnPointerClickAsObservable().AsUnitObservable();
    }
}