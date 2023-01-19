using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using RenderingOptimization;
using Domain.Chunks;

public class BlockRenderingSurfacesTest
{
    [Test]
    public void DeepCopy()
    {
        var source = new BlockRenderingSurfaces();
        var rc = new RelativeCoordinate(8, 8, 8);
        var value = new BlockRenderingSurface(Surface.Left);
        source.SetBlockRenderingSurfaceDirectly(rc, new BlockRenderingSurface(Surface.Left));

        var copy = source.DeepCopy();

        Assert.AreEqual(new BlockRenderingSurface(Surface.Left), copy.GetBlockRenderingSurface(rc));
    }
}
