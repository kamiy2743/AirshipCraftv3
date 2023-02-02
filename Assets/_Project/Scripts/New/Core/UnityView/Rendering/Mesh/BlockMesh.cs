using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityView.Rendering
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

        // TODO ChunkRender以外でもBlockMeshを共有して使うのは良くないかも
        internal readonly MeshData All;

        internal BlockMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            rightPart = ExtractPartMesh(Direction.Right, vertices, triangles, uvs);
            leftPart = ExtractPartMesh(Direction.Left, vertices, triangles, uvs);
            topPart = ExtractPartMesh(Direction.Top, vertices, triangles, uvs);
            bottomPart = ExtractPartMesh(Direction.Bottom, vertices, triangles, uvs);
            forwardPart = ExtractPartMesh(Direction.Forward, vertices, triangles, uvs);
            backPart = ExtractPartMesh(Direction.Back, vertices, triangles, uvs);

            otherPart = new MeshData(new Vector3[0], new int[0], new Vector2[0]);

            All = new MeshData(vertices, triangles, uvs);
        }

        private MeshData ExtractPartMesh(Direction direction, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            var verticesMap = new Dictionary<int, (Vector3, int)>();
            var preTriangles = new List<int>();
            var partUVs = new List<Vector2>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var t1 = triangles[i];
                var t2 = triangles[i + 1];
                var t3 = triangles[i + 2];
                var v1 = vertices[t1];
                var v2 = vertices[t2];
                var v3 = vertices[t3];

                if (IsPartPolygon(direction, v1, v2, v3))
                {
                    if (verticesMap.TryAdd(t1, (v1, verticesMap.Count)))
                    {
                        partUVs.Add(uvs[t1]);
                    }
                    if (verticesMap.TryAdd(t2, (v2, verticesMap.Count)))
                    {
                        partUVs.Add(uvs[t2]);
                    }
                    if (verticesMap.TryAdd(t3, (v3, verticesMap.Count)))
                    {
                        partUVs.Add(uvs[t3]);
                    }

                    preTriangles.Add(t1);
                    preTriangles.Add(t2);
                    preTriangles.Add(t3);
                }
            }

            var partVertices = verticesMap.Values.Select(value => value.Item1).ToArray();
            var partTriangles = preTriangles.Select(value => verticesMap[value].Item2).ToArray();

            return new MeshData(partVertices, partTriangles, partUVs.ToArray());
        }

        private bool IsPartPolygon(Direction direction, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            switch (direction)
            {
                case Direction.Right:
                    return Mathf.Approximately(v1.x, 1) && Mathf.Approximately(v2.x, 1) && Mathf.Approximately(v3.x, 1);
                case Direction.Left:
                    return Approximately0(v1.x) && Approximately0(v2.x) && Approximately0(v3.x);
                case Direction.Top:
                    return Mathf.Approximately(v1.y, 1) && Mathf.Approximately(v2.y, 1) && Mathf.Approximately(v3.y, 1);
                case Direction.Bottom:
                    return Approximately0(v1.y) && Approximately0(v2.y) && Approximately0(v3.y);
                case Direction.Forward:
                    return Mathf.Approximately(v1.z, 1) && Mathf.Approximately(v2.z, 1) && Mathf.Approximately(v3.z, 1);
                case Direction.Back:
                    return Approximately0(v1.z) && Approximately0(v2.z) && Approximately0(v3.z);
            }

            throw new Exception("実装漏れ");
        }

        // Mathf.Approximately(a, 0)だとfalseを返すことがある
        private bool Approximately0(float a)
        {
            return a < 0.01f && a > -0.01f;
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