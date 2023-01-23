using System;
using UnityEngine;

namespace RenderingOptimization
{
    internal record BlockMesh
    {
        private readonly MeshData rightPart;
        private readonly MeshData leftPart;
        private readonly MeshData topPart;
        private readonly MeshData bottomPart;
        private readonly MeshData forwardPart;
        private readonly MeshData backPart;
        internal readonly MeshData otherPart;

        internal BlockMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            rightPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
            leftPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
            topPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
            bottomPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
            forwardPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
            backPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);

            otherPart = new MeshData(vertices, triangles, uvs);
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