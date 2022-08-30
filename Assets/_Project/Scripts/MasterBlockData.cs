using UnityEngine;

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
        public BlockMeshData MeshData => meshData ??= new BlockMeshData(CubeMesh.Vertices, CubeMesh.Triangles);
    }
}