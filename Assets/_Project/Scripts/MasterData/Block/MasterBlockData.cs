using UnityEngine;
using MasterData.Util;

namespace MasterData.Block
{
    [System.Serializable]
    public class MasterBlockData
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        private BlockMeshData meshData;

        public int ID => id;
        public string Name => name;
        public BlockMeshData MeshData
        {
            get
            {
                if (meshData != null) return meshData;

                // Airの場合
                if (id == 0)
                {
                    meshData = new BlockMeshData(new Vector3[0], new int[0]);
                    return meshData;
                }

                meshData = new BlockMeshData(CubeMesh.Vertices, CubeMesh.Triangles);
                return meshData;
            }
        }
    }
}