using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using BlockSystem;

namespace Player
{
    internal class BlockOutline : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        internal void SetMesh(MeshData meshData)
        {
            var mesh = meshFilter.mesh;
            mesh.Clear();

            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();
        }

        internal void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        internal void SetPosition(BlockCoordinate bc)
        {
            transform.position = bc.ToVector3();
        }
    }
}