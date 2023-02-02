using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityView.Rendering
{
    internal class BlockMeshFactory
    {
        internal BlockMesh Create(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            var value = new MeshData(vertices, triangles, uvs);

            var rightPart = ExtractPartMesh(Direction.Right, vertices, triangles, uvs);
            var leftPart = ExtractPartMesh(Direction.Left, vertices, triangles, uvs);
            var topPart = ExtractPartMesh(Direction.Top, vertices, triangles, uvs);
            var bottomPart = ExtractPartMesh(Direction.Bottom, vertices, triangles, uvs);
            var forwardPart = ExtractPartMesh(Direction.Forward, vertices, triangles, uvs);
            var backPart = ExtractPartMesh(Direction.Back, vertices, triangles, uvs);

            var otherPart = ExtractOtherPartMesh(vertices, triangles, uvs);

            return new BlockMesh(value, rightPart, leftPart, topPart, bottomPart, forwardPart, backPart, otherPart);
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

        private MeshData ExtractOtherPartMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
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

                var isOtherPart = true;
                foreach (var direction in DirectionExt.Array)
                {
                    if (IsPartPolygon(direction, v1, v2, v3))
                    {
                        isOtherPart = false;
                        break;
                    }
                }

                if (isOtherPart)
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
    }
}