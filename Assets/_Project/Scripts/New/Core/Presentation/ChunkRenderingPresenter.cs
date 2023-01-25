using UnityView.ChunkRendering.Model.RenderingSurface;

namespace Presentation
{
    internal class ChunkRenderingPresenter
    {
        private UpdateBlockRenderingSurfaceService updateBlockRenderingSurfaceService;

        internal ChunkRenderingPresenter(UpdateBlockRenderingSurfaceService updateBlockRenderingSurfaceService)
        {
            this.updateBlockRenderingSurfaceService = updateBlockRenderingSurfaceService;
        }

        // TODO ブロックの更新を検知して描画面を更新する
    }
}