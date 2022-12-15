using System.Collections.Generic;
using UnityEngine;
using Util;
using DataObject.Block;
using BlockBehaviour.Interface;

namespace MasterData.Block
{
    public class MasterBlockData
    {
        public readonly BlockID ID;
        public readonly string Name;
        public readonly Texture2D Texture;
        public readonly MeshData MeshData;

        public IInteractedBehaviour InteractedBehaviour { get; private set; }

        public MasterBlockData(MasterBlockDataSetting setting, int blocksCount)
        {
            ID = setting.ID;
            Name = setting.Name;
            Texture = setting.Texture;

            if (ID == BlockID.Air)
            {
                MeshData = new MeshData(new Vector3[0], new int[0], new Vector2[0]);
            }
            else
            {
                MeshData = new MeshData(CubeMesh.Vertices, CubeMesh.Triangles, CreateCubeUV(blocksCount));
            }
        }

        public void SetBlockBehaviour(IBlockBehaviour blockBehaviour)
        {
            if (blockBehaviour is null) return;
            if (blockBehaviour is IInteractedBehaviour) InteractedBehaviour = (IInteractedBehaviour)blockBehaviour;
        }

        private Vector2[] CreateCubeUV(int blocksCount)
        {
            var side = Mathf.CeilToInt(Mathf.Sqrt(blocksCount));
            var x = (int)ID % side;
            var y = (int)ID / side;

            var leftBottom = new Vector2(
                (Texture.width + 2) * x + 1,
                (Texture.width + 2) * y + 1
            );

            var blockTextureSize = (Texture.width + 2) * side;
            var uvs = new List<Vector2>(24);

            for (int i = 0; i < 6; i++)
            {
                uvs.Add(leftBottom / blockTextureSize);
                uvs.Add((leftBottom + new Vector2(0, Texture.height)) / blockTextureSize);
                uvs.Add((leftBottom + new Vector2(Texture.width, Texture.height)) / blockTextureSize);
                uvs.Add((leftBottom + new Vector2(Texture.width, 0)) / blockTextureSize);
            }

            return uvs.ToArray();
        }
    }
}