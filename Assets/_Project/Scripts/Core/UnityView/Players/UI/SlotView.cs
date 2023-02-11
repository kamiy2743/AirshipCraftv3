using UnityEngine;

namespace UnityView.Players
{
    class SlotView : MonoBehaviour
    {
        [SerializeField] GameObject onSelected;
        
        internal void SetSelected(bool selected)
        {
            onSelected.SetActive(selected);
        }
    }
}