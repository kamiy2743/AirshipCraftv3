using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using UnityEngine;

namespace UnityView.Rendering
{
    class BlockMeshFactory
    {
        readonly SixFaceUVCreator sixFaceUVCreator;

        internal BlockMeshFactory(SixFaceUVCreator blockTextureUVCreator)
        {
            sixFaceUVCreator = blockTextureUVCreator;
        }

        internal BlockMesh Create(BlockType blockType, Vector3[] vertices, int[] triangles, Vector2[] originalUVs)
        {
            var uvs = sixFaceUVCreator.Create(blockType, originalUVs);

            var value = new MeshData(vertices, triangles, uvs);

            var rightFace = ExtractFaceMesh(Face.Right, vertices, triangles, uvs);
            var leftFace = ExtractFaceMesh(Face.Left, vertices, triangles, uvs);
            var topFace = ExtractFaceMesh(Face.Top, vertices, triangles, uvs);
            var bottomFace = ExtractFaceMesh(Face.Bottom, vertices, triangles, uvs);
            var frontFace = ExtractFaceMesh(Face.Front, vertices, triangles, uvs);
            var backFace = ExtractFaceMesh(Face.Back, vertices, triangles, uvs);

            var otherPart = ExtractOtherPartMesh(vertices, triangles, uvs);

            return new BlockMesh(value, rightFace, leftFace, topFace, bottomFace, frontFace, backFace, otherPart);
        }

        MeshData ExtractFaceMesh(Face direction, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            var verticesMap = new Dictionary<int, (Vector3, int)>();
            var preTriangles = new List<int>();
            var faceUVs = new List<Vector2>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var t1 = triangles[i];
                var t2 = triangles[i + 1];
                var t3 = triangles[i + 2];
                var v1 = vertices[t1];
                var v2 = vertices[t2];
                var v3 = vertices[t3];

                if (IsFacePolygon(direction, v1, v2, v3))
                {
                    if (verticesMap.TryAdd(t1, (v1, verticesMap.Count)))
                    {
                        faceUVs.Add(uvs[t1]);
                    }
                    if (verticesMap.TryAdd(t2, (v2, verticesMap.Count)))
                    {
                        faceUVs.Add(uvs[t2]);
                    }
                    if (verticesMap.TryAdd(t3, (v3, verticesMap.Count)))
                    {
                        faceUVs.Add(uvs[t3]);
                    }

                    preTriangles.Add(t1);
                    preTriangles.Add(t2);
                    preTriangles.Add(t3);
                }
            }

            var faceVertices = verticesMap.Values.Select(value => value.Item1).ToArray();
            var faceTriangles = preTriangles.Select(value => verticesMap[value].Item2).ToArray();

            return new MeshData(faceVertices, faceTriangles, faceUVs.ToArray());
        }

        bool IsFacePolygon(Face direction, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            switch (direction)
            {
                case Face.Right:
                    return Approximately1(v1.x) && Approximately1(v2.x) && Approximately1(v3.x);
                case Face.Left:
                    return Approximately0(v1.x) && Approximately0(v2.x) && Approximately0(v3.x);
                case Face.Top:
                    return Approximately1(v1.y) && Approximately1(v2.y) && Approximately1(v3.y);
                case Face.Bottom:
                    return Approximately0(v1.y) && Approximately0(v2.y) && Approximately0(v3.y);
                case Face.Front:
                    return Approximately1(v1.z) && Approximately1(v2.z) && Approximately1(v3.z);
                case Face.Back:
                    return Approximately0(v1.z) && Approximately0(v2.z) && Approximately0(v3.z);
            }

            throw new Exception("実装漏れ");
        }

        // Mathf.Approximately(a, 0)だとfalseを返すことがある
        bool Approximately0(float a)
        {
            return a < 0.01f && a > -0.01f;
        }

        bool Approximately1(float a)
        {
            return a < 1.01f && a > 0.99f;
        }

        MeshData ExtractOtherPartMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
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

                var isOtherFace = true;
                foreach (var face in FaceExt.Array)
                {
                    if (IsFacePolygon(face, v1, v2, v3))
                    {
                        isOtherFace = false;
                        break;
                    }
                }

                if (isOtherFace)
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