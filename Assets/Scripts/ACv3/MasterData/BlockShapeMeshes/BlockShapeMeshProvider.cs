using ACv3.Domain;
using UnityEngine;
using ACv3.UnityView.Rendering;

namespace ACv3.MasterData
{
    public class BlockShapeMeshProvider : IBlockShapeMeshProvider
    {
        readonly MasterBlockShapeMeshes masterBlockShapeMeshes;

        internal BlockShapeMeshProvider(MasterBlockShapeMeshes masterBlockShapeMeshes)
        {
            this.masterBlockShapeMeshes = masterBlockShapeMeshes;
        }

        public Mesh GetMesh(BlockShape blockShape)
        {
            return masterBlockShapeMeshes.BlockShapeMeshes[blockShape];
        }
    }
}