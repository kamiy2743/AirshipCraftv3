using UnityEngine;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        public ChunkData ChunkData { get; private set; }

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        internal void Initial(ChunkData chunkData)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            ChunkData = chunkData;

            meshFilter.mesh = ChunkMeshCreator.CreateMesh(chunkData);
        }
    }
}
