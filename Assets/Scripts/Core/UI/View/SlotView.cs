using UnityEngine;

namespace ACv3.UI.View
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] GameObject selectedOutline;

        public void SetSelected(bool isSelected)
        {
            selectedOutline.SetActive(isSelected);
        }
    }
}