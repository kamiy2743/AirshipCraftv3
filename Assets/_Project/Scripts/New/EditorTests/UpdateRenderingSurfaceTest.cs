using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using RenderingOptimization;
using Domain;
using Domain.Chunks;
using UseCase;
using Infrastructure;

public class UpdateRenderingSurfaceTest
{
    [Test]
    public void _8_8_8のブロックとそれと接しているブロックの描画面の更新()
    {
        var chunkFactory = new AllDirtChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory);
        var renderingSurfaceRepository = new OnMemoryChunkRenderingSurfaceRepository();
        var renderingSurfaceFactory = new ChunkRenderingSurfaceFactory(chunkProvider);
        var renderingSurfaceProvider = new ChunkRenderingSurfaceProvider(renderingSurfaceRepository, renderingSurfaceFactory);
        var updateBlockRenderingSurfaceService = new UpdateBlockRenderingSurfaceService(chunkProvider, renderingSurfaceRepository, renderingSurfaceProvider);

        var targetCoordinate = new BlockGridCoordinate(8, 8, 8);
        var targetRelativeCoordinate = RelativeCoordinate.Parse(targetCoordinate);
        var targetChunkGridCoordinate = ChunkGridCoordinate.Parse(targetCoordinate);

        var renderingSurface = renderingSurfaceProvider.GetRenderingSurface(targetChunkGridCoordinate);

        var target = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate);
        var right = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate.Add(x: 1));
        var left = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate.Add(x: -1));
        var top = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate.Add(y: 1));
        var bottom = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate.Add(y: -1));
        var forward = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate.Add(z: 1));
        var back = renderingSurface.GetBlockRenderingSurface(targetRelativeCoordinate.Add(z: -1));

        Assert.AreEqual(false, target.Contains(Surface.Left));
        Assert.AreEqual(false, target.Contains(Surface.Right));
        Assert.AreEqual(false, target.Contains(Surface.Top));
        Assert.AreEqual(false, target.Contains(Surface.Bottom));
        Assert.AreEqual(false, target.Contains(Surface.Forward));
        Assert.AreEqual(false, target.Contains(Surface.Back));

        Assert.AreEqual(false, right.Contains(Surface.Left));
        Assert.AreEqual(false, left.Contains(Surface.Right));
        Assert.AreEqual(false, top.Contains(Surface.Bottom));
        Assert.AreEqual(false, bottom.Contains(Surface.Top));
        Assert.AreEqual(false, forward.Contains(Surface.Back));
        Assert.AreEqual(false, back.Contains(Surface.Forward));

        var targetBlock = chunkProvider.GetChunk(targetChunkGridCoordinate).GetBlock(targetRelativeCoordinate);
        updateBlockRenderingSurfaceService.UpdateBlockRenderingSurface(targetCoordinate, targetBlock.blockTypeID);

        Assert.AreEqual(false, target.Contains(Surface.Left));
        Assert.AreEqual(false, target.Contains(Surface.Right));
        Assert.AreEqual(false, target.Contains(Surface.Top));
        Assert.AreEqual(false, target.Contains(Surface.Bottom));
        Assert.AreEqual(false, target.Contains(Surface.Forward));
        Assert.AreEqual(false, target.Contains(Surface.Back));

        Assert.AreEqual(false, right.Contains(Surface.Left));
        Assert.AreEqual(false, left.Contains(Surface.Right));
        Assert.AreEqual(false, top.Contains(Surface.Bottom));
        Assert.AreEqual(false, bottom.Contains(Surface.Top));
        Assert.AreEqual(false, forward.Contains(Surface.Back));
        Assert.AreEqual(false, back.Contains(Surface.Forward));
    }
}
