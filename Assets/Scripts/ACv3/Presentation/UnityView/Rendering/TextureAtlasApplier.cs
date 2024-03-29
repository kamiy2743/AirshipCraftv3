using UnityEngine;
using Zenject;

namespace ACv3.Presentation.Rendering
{
    public class TextureAtlasApplier : IInitializable
    {
        readonly Material blockMaterial;
        readonly IBlockTextureAtlas blockTextureAtlas;

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