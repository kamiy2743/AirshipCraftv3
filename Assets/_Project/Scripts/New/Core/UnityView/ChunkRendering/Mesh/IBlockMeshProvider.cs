using Domain;

namespace UnityView.ChunkRendering.Mesh
{
    internal interface IBlockMeshProvider
    {
        BlockMesh GetBlockMesh(BlockTypeID blockTypeID);
    }
}