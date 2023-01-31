using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using UnityView.ChunkRender;
using UnityView.ChunkRender.RenderingSurface;

public class BlockRenderingSurfaceTest
{
    [Test]
    public void 初期状態にRightを足すとRightになる()
    {
        var result = new BlockRenderingSurface() + Direction.Right;
        Assert.AreEqual((byte)Direction.Right, result.SurfaceByteDebug);
    }

    [Test]
    public void RightにLeftを足すとRight_Leftになる()
    {
        var result = new BlockRenderingSurface() + Direction.Right + Direction.Left;
        Assert.AreEqual((byte)Direction.Right + (byte)Direction.Left, result.SurfaceByteDebug);
    }

    [Test]
    public void 複数のSurfaceを渡すコンストラクタ_Right_Left()
    {
        var result = new BlockRenderingSurface(Direction.Right, Direction.Left);
        Assert.AreEqual((byte)Direction.Right + (byte)Direction.Left, result.SurfaceByteDebug);
    }

    [Test]
    public void Right_LeftにContains_Rightを呼ぶとTrueになる()
    {
        Assert.AreEqual(true, new BlockRenderingSurface(Direction.Right, Direction.Left).Contains(Direction.Right));
    }

    [Test]
    public void Right_LeftにContains_Leftを呼ぶとTrueになる()
    {
        Assert.AreEqual(true, new BlockRenderingSurface(Direction.Right, Direction.Left).Contains(Direction.Left));
    }
}
