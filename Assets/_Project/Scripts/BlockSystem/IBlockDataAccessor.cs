using UnityEngine;

namespace BlockSystem
{
    public interface IBlockDataAccessor
    {
        BlockData GetBlockData(Vector3 position);
    }
}
