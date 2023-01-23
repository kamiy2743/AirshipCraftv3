using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using RenderingDomain;
using RenderingDomain.RenderingSurface;
using Domain.Chunks;

public class BlockRenderingSurfacesTest
{
    [Test]
    public void DeepCopy()
    {
        var source = new BlockRenderingSurfaces();
        var rc = new RelativeCoordinate(8, 8, 8);
        var value = new BlockRenderingSurface(Direction.Left);
        source.SetBlockRenderingSurfaceDirectly(rc, new BlockRenderingSurface(Direction.Left));

        var copy = source.DeepCopy();

        Assert.AreEqual(new BlockRenderingSurface(Direction.Left), copy.GetBlockRenderingSurface(rc));
    }
}
