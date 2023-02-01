using Unity.Mathematics;

namespace UseCase
{
    public class BreakBlockUseCase
    {
        public void BreakBlock(float3 position)
        {
            UnityEngine.Debug.Log("break: " + position);
        }
    }
}