using Domain;
using UnityEngine;

namespace UnityView.Rendering
{
    public interface IBlockShapeMeshProvider
    {
        Mesh GetMesh(BlockShape blockShape);
    }
}