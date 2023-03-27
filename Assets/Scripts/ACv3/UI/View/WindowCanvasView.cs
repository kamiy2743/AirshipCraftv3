using ACv3.Domain.Items;
using UnityEngine;

namespace ACv3.UI.View
{
    public class WindowCanvasView : MonoBehaviour
    {
        [SerializeField] GrabbingInventoryItemView grabbingInventoryItemView;
        
        public void Open() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);

        public void SetGrabbingItemActive(bool isActive) => grabbingInventoryItemView.SetActive(isActive);
        public void SetGrabbingItemPosition(Vector2 position) => grabbingInventoryItemView.SetPosition(position);
        public void SetGrabbingItem(Texture2D texture, Amount amount, string debugItemId) => grabbingInventoryItemView.SetItem(texture, amount, debugItemId);
    }
}