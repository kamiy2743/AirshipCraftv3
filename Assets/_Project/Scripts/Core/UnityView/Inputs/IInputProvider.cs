using UnityEngine;

namespace UnityView.Inputs
{
    interface IInputProvider
    {
        Vector3 DebugFly();
        bool PlaceBlock();
        bool BreakBlock();
    }
}