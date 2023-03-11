using UnityEngine;

namespace UnityView.Inputs
{
    public interface IInputProvider
    {
        Vector3 DebugFly();
        bool PlaceBlock();
        bool BreakBlock();
    }
}