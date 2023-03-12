using System;
using System.Collections.Generic;
using ACv3.Domain.Inventories;
using ACv3.Domain.Items;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ACv3.UI.View
{
    public class PlayerInventoryView : MonoBehaviour
    {
        [SerializeField] PlayerInventorySlots slots;

        readonly Dictionary<PlayerInventorySlotId, SlotView> slotViews = new();

        readonly Subject<PlayerInventorySlotId> onCursorSlotSubject = new();
        readonly Subject<Unit> onUnCursorSlotSubject = new();

        public IObservable<PlayerInventorySlotId> OnCursorSlot => onCursorSlotSubject;
        public IObservable<Unit> OnUnCursorSlot => onUnCursorSlotSubject;

        public void Initialize()
        {
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(0, i, slots.line0[i]);
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(1, i, slots.line1[i]);
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(2, i, slots.line2[i]);
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(3, i, slots.line3[i]);

            void SetUp(int line, int row, SlotView slotView)
            {
                var slotId = new PlayerInventorySlotId(line, row);
                slotViews[slotId] = slotView;
                
                slotView.GetComponent<ObservablePointerEnterTrigger>().OnPointerEnterAsObservable()
                    .Subscribe(_ => onCursorSlotSubject.OnNext(slotId))
                    .AddTo(this);
                
                slotView.GetComponent<ObservablePointerExitTrigger>().OnPointerExitAsObservable()
                    .Subscribe(_ => onUnCursorSlotSubject.OnNext(Unit.Default))
                    .AddTo(this);
            }
        }

        public void SetSelectedSlot(PlayerInventorySlotId slotId)
        {
            foreach (var id in slotViews.Keys)
            {
                slotViews[id].SetSelected(id == slotId);
            }
        }

        public void DeselectSlot()
        {
            foreach (var slotView in slotViews.Values)
            {
                slotView.SetSelected(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            DeselectSlot();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetItem(PlayerInventorySlotId slotId, Texture2D texture, Amount amount)
        {
            slotViews[slotId].SetItem(texture, amount);
        }
        
        [Serializable]
        record PlayerInventorySlots
        {
            public SlotView[] line0;
            public SlotView[] line1;
            public SlotView[] line2;
            public SlotView[] line3;
        }
    }
}