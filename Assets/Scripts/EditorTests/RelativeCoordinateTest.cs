using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ACv3.Domain;
using ACv3.Domain.Chunks;

public class RelativeCoordinateTest
{
    [Test]
    public void BlockCoordinate_0_0_0をRelativeCoordinate_0_0_0に変換する()
    {
        Assert.AreEqual(new RelativeCoordinate(0, 0, 0), RelativeCoordinate.Parse(new BlockGridCoordinate(0, 0, 0)));
    }

    [Test]
    public void BlockCoordinate_15_15_15をRelativeCoordinate_15_15_15に変換する()
    {
        Assert.AreEqual(new RelativeCoordinate(15, 15, 15), RelativeCoordinate.Parse(new BlockGridCoordinate(15, 15, 15)));
    }

    [Test]
    public void BlockCoordinate_16_16_16をRelativeCoordinate_0_0_0に変換する()
    {
        Assert.AreEqual(new RelativeCoordinate(0, 0, 0), RelativeCoordinate.Parse(new BlockGridCoordinate(16, 16, 16)));
    }

    [Test]
    public void BlockCoordinate_m1_m1_m1をRelativeCoordinate_15_15_15に変換する()
    {
        Assert.AreEqual(new RelativeCoordinate(15, 15, 15), RelativeCoordinate.Parse(new BlockGridCoordinate(-1, -1, -1)));
    }

    [Test]
    public void BlockCoordinate_m16_m16_m16をRelativeCoordinate_0_0_0に変換する()
    {
        Assert.AreEqual(new RelativeCoordinate(0, 0, 0), RelativeCoordinate.Parse(new BlockGridCoordinate(-16, -16, -16)));
    }

    [Test]
    public void BlockCoordinate_m17_m17_m17をRelativeCoordinate_15_15_15に変換する()
    {
        Assert.AreEqual(new RelativeCoordinate(15, 15, 15), RelativeCoordinate.Parse(new BlockGridCoordinate(-17, -17, -17)));
    }
}
