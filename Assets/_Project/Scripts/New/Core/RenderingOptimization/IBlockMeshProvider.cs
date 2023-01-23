using Domain;

namespace RenderingOptimization
{
    internal interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockTypeID blockTypeID);
    }
}