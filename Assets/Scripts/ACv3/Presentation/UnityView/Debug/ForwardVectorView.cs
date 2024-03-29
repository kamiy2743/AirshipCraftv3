using TMPro;
using UnityEngine;
using ACv3.Presentation.Players;
using Zenject;

namespace ACv3.Presentation.Debug
{
    class ForwardVectorView : MonoBehaviour
    {
        [Inject] PlayerCamera playerCamera;

        [SerializeField] TextMeshProUGUI text;

        void Update()
        {
            var xColor = "black";
            if (playerCamera.Forward.x >= 0.5f) xColor = "red";
            if (playerCamera.Forward.x <= -0.5f) xColor = "blue";
            var x = $"<color=\"{xColor}\">{playerCamera.Forward.x:F1}</color>";

            var yColor = "black";
            if (playerCamera.Forward.y >= 0.5f) yColor = "red";
            if (playerCamera.Forward.y <= -0.5f) yColor = "blue";
            var y = $"<color=\"{yColor}\">{playerCamera.Forward.y:F1}</color>";

            var zColor = "black";
            if (playerCamera.Forward.z >= 0.5f) zColor = "red";
            if (playerCamera.Forward.z <= -0.5f) zColor = "blue";
            var z = $"<color=\"{zColor}\">{playerCamera.Forward.z:F1}</color>";

            text.text = $"Forward: ({x}, {y}, {z})";
        }
    }
}
