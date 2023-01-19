using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using RenderingOptimization;

public class BlockRenderingSurfaceTest
{
    [Test]
    public void 初期状態にRightを足すとRightになる()
    {
        var result = new BlockRenderingSurface() + Surface.Right;
        Assert.AreEqual((byte)Surface.Right, result.SurfaceByteDebug);
    }

    [Test]
    public void RightにLeftを足すとRight_Leftになる()
    {
        var result = new BlockRenderingSurface() + Surface.Right + Surface.Left;
        Assert.AreEqual((byte)Surface.Right + (byte)Surface.Left, result.SurfaceByteDebug);
    }

    [Test]
    public void 複数のSurfaceを渡すコンストラクタ_Right_Left()
    {
        var result = new BlockRenderingSurface(Surface.Right, Surface.Left);
        Assert.AreEqual((byte)Surface.Right + (byte)Surface.Left, result.SurfaceByteDebug);
    }

    [Test]
    public void Right_LeftにContains_Right_を呼ぶとTrueになる()
    {
        Assert.AreEqual(true, new BlockRenderingSurface(Surface.Right, Surface.Left).Contains(Surface.Right));
    }
}
