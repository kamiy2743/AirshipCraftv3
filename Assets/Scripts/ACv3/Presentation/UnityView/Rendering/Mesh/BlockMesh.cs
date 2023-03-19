using System.Collections.Generic;

namespace ACv3.Presentation.Rendering
{
    record BlockMesh
    {
        internal readonly MeshData value;
        readonly Dictionary<Face, MeshData> faceMeshes;
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

            faceMeshes = new Dictionary<Face, MeshData>(FaceExt.ElemCount)
            {
                [Face.Right] = rightFace,
                [Face.Left] = leftFace,
                [Face.Top] = topFace,
                [Face.Bottom] = bottomFace,
                [Face.Front] = frontFace,
                [Face.Back] = backFace
            };
        }

        internal MeshData GetFaceMesh(Face face) => faceMeshes[face];
    }
}