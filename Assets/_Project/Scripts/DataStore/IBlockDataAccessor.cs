using UnityEngine;
using System.Threading;
using DataObject.Block;

namespace DataStore
{
    public interface IBlockDataAccessor
    {
        BlockData GetBlockData(Vector3 position, CancellationToken ct);
    }
}
