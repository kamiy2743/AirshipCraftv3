using UnityEngine;
using System.Threading;

namespace DataObject.Block
{
    public interface IBlockDataAccessor
    {
        BlockData GetBlockData(Vector3 position, CancellationToken ct);
    }
}
