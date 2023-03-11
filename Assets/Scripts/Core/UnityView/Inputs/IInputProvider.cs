using UnityEngine;

namespace ACv3.UnityView.Inputs
{
    public interface IInputProvider
    {
        Vector3 DebugFly();
        bool PlaceBlock();
        bool BreakBlock();
    }
}