using UnityEngine;
using Zenject;
using UnityView.Players;
using TMPro;

namespace UnityView.Debug
{
    internal class FocusedBlockDebugViewer : MonoBehaviour
    {
        [Inject] private FocusedBlockInfoProvider focusedBlockInfoProvider;

        [SerializeField] private TextMeshProUGUI text;

        private void Update()
        {
            if (focusedBlockInfoProvider.TryGetFocusedBlockInfo(out var focusedBlockInfo))
            {
                text.text = $"Focused: {focusedBlockInfo.pivotCoordinate}, {focusedBlockInfo.blockTypeID}";
            }
            else
            {
                text.text = $"Focused: none";
            }
        }
    }
}