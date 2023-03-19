using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ACv3.Presentation.Rendering;
using ACv3.Presentation.Rendering.Chunks;

public class BlockSurfaceTest
{
    [Test]
    public void 初期状態にRightを足すとRightになる()
    {
        var result = new BlockSurface() + Face.Right;
        Assert.AreEqual((byte)Face.Right, result.SurfaceByteDebug);
    }

    [Test]
    public void RightにLeftを足すとRight_Leftになる()
    {
        var result = new BlockSurface() + Face.Right + Face.Left;
        Assert.AreEqual((byte)Face.Right + (byte)Face.Left, result.SurfaceByteDebug);
    }

    [Test]
    public void 複数のSurfaceを渡すコンストラクタ_Right_Left()
    {
        var result = new BlockSurface(Face.Right, Face.Left);
        Assert.AreEqual((byte)Face.Right + (byte)Face.Left, result.SurfaceByteDebug);
    }

    [Test]
    public void Right_LeftにContains_Rightを呼ぶとTrueになる()
    {
        Assert.AreEqual(true, new BlockSurface(Face.Right, Face.Left).Contains(Face.Right));
    }

    [Test]
    public void Right_LeftにContains_Leftを呼ぶとTrueになる()
    {
        Assert.AreEqual(true, new BlockSurface(Face.Right, Face.Left).Contains(Face.Left));
    }
}
