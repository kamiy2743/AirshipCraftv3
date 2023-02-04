using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    public interface IBlockShapeMeshProvider
    {
        Mesh GetMesh(BlockShape blockShape);
    }
}