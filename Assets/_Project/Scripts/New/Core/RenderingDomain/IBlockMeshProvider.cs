using Domain;

namespace RenderingDomain
{
    internal interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockTypeID blockTypeID);
    }
}