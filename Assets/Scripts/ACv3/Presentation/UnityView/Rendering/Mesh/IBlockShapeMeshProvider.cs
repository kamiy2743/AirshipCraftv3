using ACv3.Domain;
using UnityEngine;

namespace ACv3.Presentation.Rendering
{
    public interface IBlockShapeMeshProvider
    {
        Mesh GetMesh(BlockShape blockShape);
    }
}