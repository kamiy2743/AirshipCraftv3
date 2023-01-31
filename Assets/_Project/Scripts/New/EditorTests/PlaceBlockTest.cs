using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Domain;
using Domain.Chunks;
using Infrastructure;
using UseCase;
using UnityView.ChunkRender.Surfaces;

public class PlaceBlockTest
{
    [Test]
    public void BlockGridCoordinate_0_0_0にGrassを設置()
    {
        var chunkRepository = new OnMemoryChunkRepository();
        var chunkFactory = new AllDirtChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory, chunkRepository);
        var placeBlockUseCase = new PlaceBlockUseCase(chunkRepository, chunkProvider);

        var placePosition = new Vector3(0, 0, 0);
        placeBlockUseCase.PlaceBlock(placePosition, BlockTypeID.Grass);

        var bgc = new BlockGridCoordinate(placePosition);
        var cgc = ChunkGridCoordinate.Parse(bgc);
        var rc = RelativeCoordinate.Parse(bgc);
        var block = chunkProvider.GetChunk(cgc).GetBlock(rc);

        Assert.AreEqual(BlockTypeID.Grass, block.blockTypeID);
    }

    [Test]
    public void BlockGridCoordinate_0_0_0にDirtを設置()
    {
        var chunkRepository = new OnMemoryChunkRepository();
        var chunkFactory = new AllDirtChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory, chunkRepository);
        var placeBlockUseCase = new PlaceBlockUseCase(chunkRepository, chunkProvider);

        var placePosition = new Vector3(0, 0, 0);
        placeBlockUseCase.PlaceBlock(placePosition, BlockTypeID.Dirt);

        var bgc = new BlockGridCoordinate(placePosition);
        var cgc = ChunkGridCoordinate.Parse(bgc);
        var rc = RelativeCoordinate.Parse(bgc);
        var block = chunkProvider.GetChunk(cgc).GetBlock(rc);

        Assert.AreEqual(BlockTypeID.Dirt, block.blockTypeID);
    }
}
