using System;
using Domain;

namespace UnityView.Rendering
{
    internal record BlockMesh
    {
        internal readonly MeshData value;

        private readonly MeshData rightPart;
        private readonly MeshData leftPart;
        private readonly MeshData topPart;
        private readonly MeshData bottomPart;
        private readonly MeshData forwardPart;
        private readonly MeshData backPart;

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
            this.rightPart = rightPart;
            this.leftPart = leftPart;
            this.topPart = topPart;
            this.bottomPart = bottomPart;
            this.forwardPart = forwardPart;
            this.backPart = backPart;
            this.otherPart = otherPart;
        }

        internal MeshData GetPartMesh(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return rightPart;
                case Direction.Left:
                    return leftPart;
                case Direction.Top:
                    return topPart;
                case Direction.Bottom:
                    return bottomPart;
                case Direction.Forward:
                    return forwardPart;
                case Direction.Back:
                    return backPart;
            }

            throw new Exception("実装漏れ");
        }
    }
}