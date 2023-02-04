using System;
using System.Collections.Generic;
using Domain;

namespace UnityView.Rendering
{
    internal record BlockMesh
    {
        internal readonly MeshData value;
        private Dictionary<Direction, MeshData> partMeshes;
        internal readonly MeshData otherPart;

        internal BlockMesh(
            MeshData value,
            MeshData rightPart,
            MeshData leftPart,
            MeshData topPart,
            MeshData bottomPart,
            MeshData forwardPart,
            MeshData backPart,
            MeshData otherPart)
        {
            this.value = value;
            this.otherPart = otherPart;

            partMeshes = new Dictionary<Direction, MeshData>(DirectionExt.ElemCount);
            partMeshes[Direction.Right] = rightPart;
            partMeshes[Direction.Left] = leftPart;
            partMeshes[Direction.Top] = topPart;
            partMeshes[Direction.Bottom] = bottomPart;
            partMeshes[Direction.Forward] = forwardPart;
            partMeshes[Direction.Back] = backPart;
        }

        internal MeshData GetPartMesh(Direction direction) => partMeshes[direction];
    }
}