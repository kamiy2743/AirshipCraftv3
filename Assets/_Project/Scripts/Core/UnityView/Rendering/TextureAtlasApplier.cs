using UnityEngine;
using Zenject;

namespace UnityView.Rendering
{
    class TextureAtlasApplier : IInitializable
    {
        readonly Material _blockMaterial;
        readonly IBlockTextureAtlas _blockTextureAtlas;

        internal TextureAtlasApplier(Material blockMaterial, IBlockTextureAtlas blockTextureAtlas)
        {
            _blockMaterial = blockMaterial;
            _blockTextureAtlas = blockTextureAtlas;
        }

        public void Initialize()
        {
            _blockMaterial.mainTexture = _blockTextureAtlas.GetAtlas();
        }
    }
}