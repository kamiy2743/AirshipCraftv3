using UnityEngine;
using Zenject;

namespace UnityView.Rendering
{
    internal class TextureAtlasApplier : IInitializable
    {
        private Material blockMaterial;
        private IBlockTextureAtlas blockTextureAtlas;

        internal TextureAtlasApplier(Material blockMaterial, IBlockTextureAtlas blockTextureAtlas)
        {
            this.blockMaterial = blockMaterial;
            this.blockTextureAtlas = blockTextureAtlas;
        }

        public void Initialize()
        {
            blockMaterial.mainTexture = blockTextureAtlas.GetAtlas();
        }
    }
}