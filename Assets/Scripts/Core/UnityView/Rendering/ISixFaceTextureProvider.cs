using Domain;

namespace UnityView.Rendering
{
    public interface ISixFaceTextureProvider
    {
        bool TryGetSixFaceTexture(BlockType blockType, out SixFaceTexture result);
    }
}