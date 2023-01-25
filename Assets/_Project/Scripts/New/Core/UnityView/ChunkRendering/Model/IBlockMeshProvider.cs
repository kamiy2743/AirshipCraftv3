using Domain;

namespace UnityView.ChunkRendering.Model
{
    internal interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockTypeID blockTypeID);
    }
}