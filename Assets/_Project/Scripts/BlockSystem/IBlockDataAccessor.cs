using UnityEngine;
using System.Threading;

namespace BlockSystem
{
    public interface IBlockDataAccessor
    {
        BlockData GetBlockData(Vector3 position, CancellationToken ct);
    }
}
