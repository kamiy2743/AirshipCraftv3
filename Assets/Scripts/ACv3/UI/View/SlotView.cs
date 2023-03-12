using ACv3.Domain.Items;
using TMPro;
using UnityEngine;

namespace ACv3.UI.View
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] GameObject selectedOutline;
        [SerializeField] TextMeshProUGUI amountText;

        public void SetSelected(bool isSelected)
        {
            selectedOutline.SetActive(isSelected);
        }

        public void SetItem(Texture2D texture, Amount amount)
        {
            if (amount > new Amount(1))
            {
                amountText.text = amount.ToStringValue();
            }
            else
            {
                amountText.text = "";
            }
        }
    }
}