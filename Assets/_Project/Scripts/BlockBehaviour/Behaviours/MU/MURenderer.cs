using System;
using UnityEngine;
using Util;

namespace BlockBehaviour
{
    public class MURenderer : MonoBehaviour, IDisposable
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        private Mesh mesh;

        /// <summary> 
        /// メインスレッドのみ 
        /// </summary>
        internal void SetMesh(NativeMeshData meshData)
        {
            if (mesh is null)
            {
                mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.RecalculateNormals();
        }

        public void Dispose()
        {
            if (mesh is not null)
            {
                Destroy(mesh);
            }
        }
    }
}
