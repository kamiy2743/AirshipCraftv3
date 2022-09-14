using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class CubeTest : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;

    void Start()
    {
        var mesh = new Mesh();

        mesh.SetVertices(CubeMesh.Vertices);
        mesh.SetTriangles(CubeMesh.Triangles, 0);
        mesh.SetUVs(0, CubeMesh.UVs);
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
