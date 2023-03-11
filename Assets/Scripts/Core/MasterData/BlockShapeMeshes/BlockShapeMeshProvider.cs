using Domain;
using UnityEngine;
using UnityView.Rendering;

namespace MasterData
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