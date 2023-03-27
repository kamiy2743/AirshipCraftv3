using System;
using System.Collections.Generic;
using ACv3.Domain.Inventories;
using ACv3.Domain.Items;
using UniRx;
using UnityEngine;

namespace ACv3.UI.View
{
    public class PlayerWindowView : MonoBehaviour
    {
        [SerializeField] PlayerInventorySlots slots;

        readonly Dictionary<PlayerInventorySlotId, InteractableSlotView> slotViews = new();

        readonly Subject<PlayerInventorySlotId> onCursorSlotSubject = new();
        readonly Subject<Unit> onUnCursorSlotSubject = new();
        readonly Subject<PlayerInventorySlotId> onClickSlotSubject = new();

        public IObservable<PlayerInventorySlotId> OnCursorSlot => onCursorSlotSubject;
        public IObservable<Unit> OnUnCursorSlot => onUnCursorSlotSubject;
        public IObservable<PlayerInventorySlotId> OnClickSlot => onClickSlotSubject;

        public void Initialize()
        {
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(0, i, slots.line0[i]);
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(1, i, slots.line1[i]);
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(2, i, slots.line2[i]);
            for (int i = 0; i < PlayerInventorySlotId.RowCount; i++) SetUp(3, i, slots.line3[i]);

            void SetUp(int line, int row, InteractableSlotView slotView)
            {
                var slotId = new PlayerInventorySlotId(line, row);
                slotViews[slotId] = slotView;
                
                slotView.OnCursored
                    .Subscribe(_ => onCursorSlotSubject.OnNext(slotId))
                    .AddTo(this);
                
                slotView.OnUnCursored
                    .Subscribe(_ => onUnCursorSlotSubject.OnNext(Unit.Default))
                    .AddTo(this);
                
                slotView.OnClocked
                    .Subscribe(_ => onClickSlotSubject.OnNext(slotId))
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

        public void Open()
        {
            gameObject.SetActive(true);
            DeselectSlot();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void SetItem(PlayerInventorySlotId slotId, Texture2D texture, Amount amount, string debugItemId)
        {
            slotViews[slotId].SetItem(texture, amount, debugItemId);
        }
        
        [Serializable]
        record PlayerInventorySlots
        {
            public InteractableSlotView[] line0;
            public InteractableSlotView[] line1;
            public InteractableSlotView[] line2;
            public InteractableSlotView[] line3;
        }
    }
}