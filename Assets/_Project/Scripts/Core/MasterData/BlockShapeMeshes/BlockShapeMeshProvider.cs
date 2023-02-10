using UnityEngine;
using Domain;
using UnityView.Rendering;

namespace MasterData
{
    class BlockShapeMeshProvider : IBlockShapeMeshProvider
    {
        readonly MasterBlockShapeMeshes _masterBlockShapeMeshes;

        internal BlockShapeMeshProvider(MasterBlockShapeMeshes masterBlockShapeMeshes)
        {
            _masterBlockShapeMeshes = masterBlockShapeMeshes;
        }

        public Mesh GetMesh(BlockShape blockShape)
        {
            return _masterBlockShapeMeshes.BlockShapeMeshes[blockShape];
        }
    }
}