using System;
using System.Collections.Generic;
using Domain.Players;
using UniRx;
using UnityEngine;
using UnityView.Inputs;
using Zenject;

namespace UnityView.Players
{
    public class ItemBarView : MonoBehaviour
    {
        [SerializeField] List<SlotView> slots;
        
        [Inject] IInputProvider _inputProvider;

        public IObservable<ItemBarScrollType> OnScrolled { get; private set; }
        
        void Awake()
        {
            var connectableObservable = Observable
                .EveryUpdate()
                .Select(_ => _inputProvider.ItemBarScroll())
                .Where(x => x != ItemBarScrollType.None)
                .Publish();

            connectableObservable.Connect();
            OnScrolled = connectableObservable;
        }

        public void SetSelectedSlot(ItemBarSlotID slotID)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].SetSelected(i == slotID.ToInt());
            }
        }
    }
}
