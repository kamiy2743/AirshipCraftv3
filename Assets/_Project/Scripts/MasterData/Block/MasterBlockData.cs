using UnityEngine;
using Util;

namespace MasterData.Block
{
    [System.Serializable]
    public class MasterBlockData
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private Texture2D texture;
        private MeshData meshData;

        public int ID => id;
        public string Name => name;
        public Texture2D Texture => texture;
        public MeshData MeshData
        {
            get
            {
                if (meshData != null) return meshData;

                // Airの場合
                if (id == 0)
                {
                    meshData = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
                    return meshData;
                }

                meshData = new MeshData(CubeMesh.Vertices, CubeMesh.Triangles, CubeMesh.UVs);
                return meshData;
            }
        }
    }
}