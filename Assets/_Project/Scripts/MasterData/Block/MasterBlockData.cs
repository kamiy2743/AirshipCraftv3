using System;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DataObject.Block;
using UnityEditor;

namespace MasterData.Block
{
    [System.Serializable]
    public class MasterBlockData
    {
        [SerializeField] private string name;
        [SerializeField] private Texture2D texture;
        [SerializeField] private MonoScript behaviour;

        public BlockID ID { get; private set; }
        public string Name => name;
        public Texture2D Texture => texture;
        public MeshData MeshData { get; private set; }

        public IInteractedBehaviour _interactedBehaviour;
        public IInteractedBehaviour InteractedBehaviour
        {
            get
            {
                if (behaviour is null) return null;
                if (_interactedBehaviour is not null) return _interactedBehaviour;
                _interactedBehaviour = (IInteractedBehaviour)Activator.CreateInstance(behaviour.GetClass());
                return _interactedBehaviour;
            }
        }

        internal void Init(int index, int blockCount)
        {
            ID = (BlockID)System.Enum.Parse(typeof(BlockID), name);

            if (ID == BlockID.Air)
            {
                MeshData = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
                return;
            }

            MeshData = new MeshData(CubeMesh.Vertices, CubeMesh.Triangles, CreateCubeUV(index, blockCount));
        }

        private Vector2[] CreateCubeUV(int index, int blockCount)
        {
            var side = Mathf.CeilToInt(Mathf.Sqrt(blockCount));
            var x = index % side;
            var y = index / side;

            var leftBottom = new Vector2(
                (texture.width + 2) * x + 1,
                (texture.width + 2) * y + 1
            );

            var blockTextureSize = (texture.width + 2) * side;
            var uvs = new List<Vector2>(24);

            for (int i = 0; i < 6; i++)
            {
                uvs.Add(leftBottom / blockTextureSize);
                uvs.Add((leftBottom + new Vector2(0, texture.height)) / blockTextureSize);
                uvs.Add((leftBottom + new Vector2(texture.width, texture.height)) / blockTextureSize);
                uvs.Add((leftBottom + new Vector2(texture.width, 0)) / blockTextureSize);
            }

            return uvs.ToArray();
        }
    }
}