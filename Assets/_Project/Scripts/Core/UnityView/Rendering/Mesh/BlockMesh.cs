using System;
using System.Collections.Generic;
using Domain;

namespace UnityView.Rendering
{
    internal record BlockMesh
    {
        internal readonly MeshData value;
        private Dictionary<Face, MeshData> faceMeshes;
        internal readonly MeshData otherPart;

        internal BlockMesh(
            MeshData value,
            MeshData rightFace,
            MeshData leftFace,
            MeshData topFace,
            MeshData bottomFace,
            MeshData frontFace,
            MeshData backFace,
            MeshData otherPart)
        {
            this.value = value;
            this.otherPart = otherPart;

            faceMeshes = new Dictionary<Face, MeshData>(FaceExt.ElemCount);
            faceMeshes[Face.Right] = rightFace;
            faceMeshes[Face.Left] = leftFace;
            faceMeshes[Face.Top] = topFace;
            faceMeshes[Face.Bottom] = bottomFace;
            faceMeshes[Face.Front] = frontFace;
            faceMeshes[Face.Back] = backFace;
        }

        internal MeshData GetFaceMesh(Face face) => faceMeshes[face];
    }
}