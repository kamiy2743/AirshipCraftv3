using System;
using System.Collections.Generic;
using Domain;

namespace UnityView.Rendering
{
    record BlockMesh
    {
        internal readonly MeshData Value;
        readonly Dictionary<Face, MeshData> _faceMeshes;
        internal readonly MeshData OtherPart;

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
            Value = value;
            OtherPart = otherPart;

            _faceMeshes = new Dictionary<Face, MeshData>(FaceExt.ElemCount);
            _faceMeshes[Face.Right] = rightFace;
            _faceMeshes[Face.Left] = leftFace;
            _faceMeshes[Face.Top] = topFace;
            _faceMeshes[Face.Bottom] = bottomFace;
            _faceMeshes[Face.Front] = frontFace;
            _faceMeshes[Face.Back] = backFace;
        }

        internal MeshData GetFaceMesh(Face face) => _faceMeshes[face];
    }
}