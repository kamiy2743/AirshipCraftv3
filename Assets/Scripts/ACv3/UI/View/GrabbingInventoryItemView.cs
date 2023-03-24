using ACv3.Domain.Items;
using TMPro;
using UnityEngine;

namespace ACv3.UI.View
{
    public class GrabbingInventoryItemView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] TextMeshProUGUI debugItemIdText;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
        public void SetPosition(Vector2 position) => transform.position = position;
        
        public void SetItem(Texture2D texture, Amount amount, string debugItemId)
        {
            if (amount > new Amount(1))
            {
                amountText.text = amount.RawString();
            }
            else
            {
                amountText.text = "";
            }

            debugItemIdText.text = debugItemId;
        }
    }
}