using TMPro;
using UnityEngine;
using ACv3.UnityView.Players;
using Zenject;

namespace ACv3.UnityView.Debug
{
    class FocusedBlockDebugViewer : MonoBehaviour
    {
        [Inject] FocusedBlockInfoProvider focusedBlockInfoProvider;

        [SerializeField] TextMeshProUGUI text;

        void Update()
        {
            if (focusedBlockInfoProvider.TryGetFocusedBlockInfo(out var focusedBlockInfo))
            {
                text.text = $"Focused: {focusedBlockInfo.pivotCoordinate}, {focusedBlockInfo.blockType}";
            }
            else
            {
                text.text = $"Focused: none";
            }
        }
    }
}