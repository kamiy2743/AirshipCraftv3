using UnityEngine;
using Zenject;
using UnityView.Players;
using TMPro;

namespace UnityView.Debug
{
    class FocusedBlockDebugViewer : MonoBehaviour
    {
        [Inject] FocusedBlockInfoProvider focusedBlockInfoProvider;

        [SerializeField] TextMeshProUGUI text;

        void Update()
        {
            if (focusedBlockInfoProvider.TryGetFocusedBlockInfo(out var focusedBlockInfo))
            {
                text.text = $"Focused: {focusedBlockInfo.PivotCoordinate}, {focusedBlockInfo.BlockType}";
            }
            else
            {
                text.text = $"Focused: none";
            }
        }
    }
}