using ACv3.Domain;

namespace ACv3.Presentation.Rendering
{
    public interface ISixFaceTextureProvider
    {
        bool TryGetSixFaceTexture(BlockType blockType, out SixFaceTexture result);
    }
}