using UnityEngine;
using Domain;
using UnityView.Rendering;

namespace MasterData
{
    internal class BlockShapeMeshProvider : IBlockShapeMeshProvider
    {
        private MasterBlockShapeMeshes masterBlockShapeMeshes;

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