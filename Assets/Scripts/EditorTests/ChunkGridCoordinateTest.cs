using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Domain;
using Domain.Chunks;

public class ChunkGridCoordinateTest
{
    [Test]
    public void BlockGridCoordinate_0_0_0をChunkGridCoordinate_0_0_0に変換()
    {
        Assert.AreEqual(new ChunkGridCoordinate(0, 0, 0), ChunkGridCoordinate.Parse(new BlockGridCoordinate(0, 0, 0)));
    }

    [Test]
    public void BlockGridCoordinate_15_15_15をChunkGridCoordinate_0_0_0に変換()
    {
        Assert.AreEqual(new ChunkGridCoordinate(0, 0, 0), ChunkGridCoordinate.Parse(new BlockGridCoordinate(15, 15, 15)));
    }

    [Test]
    public void BlockGridCoordinate_16_16_16をChunkGridCoordinate_1_1_1に変換()
    {
        Assert.AreEqual(new ChunkGridCoordinate(1, 1, 1), ChunkGridCoordinate.Parse(new BlockGridCoordinate(16, 16, 16)));
    }

    [Test]
    public void BlockGridCoordinate_m1_m1_m1をChunkGridCoordinate_m1_m1_m1に変換()
    {
        Assert.AreEqual(new ChunkGridCoordinate(-1, -1, -1), ChunkGridCoordinate.Parse(new BlockGridCoordinate(-1, -1, -1)));
    }

    [Test]
    public void BlockGridCoordinate_m16_m16_m16をChunkGridCoordinate_m1_m1_m1に変換()
    {
        Assert.AreEqual(new ChunkGridCoordinate(-1, -1, -1), ChunkGridCoordinate.Parse(new BlockGridCoordinate(-16, -16, -16)));
    }

    [Test]
    public void BlockGridCoordinate_m17_m17_m17をChunkGridCoordinate_m2_m2_m2に変換()
    {
        Assert.AreEqual(new ChunkGridCoordinate(-2, -2, -2), ChunkGridCoordinate.Parse(new BlockGridCoordinate(-17, -17, -17)));
    }
}
