using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ACv3.Domain;
using ACv3.Domain.Chunks;
using ACv3.Infrastructure;
using ACv3.UseCase;

public class PlaceBlockTest
{
    [Test]
    public void BlockGridCoordinate_0_0_0にGrassを設置()
    {
        var chunkRepository = new OnMemoryChunkRepository();
        var chunkFactory = new AllDirtChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory, chunkRepository);
        var chunkBlockSetter = new ChunkBlockSetter(chunkRepository, chunkProvider);
        var placeBlockUseCase = new PlaceBlockUseCase(chunkBlockSetter);

        var placePosition = new Vector3(0, 0, 0);
        placeBlockUseCase.PlaceBlock(placePosition, BlockType.Grass);

        var bgc = new BlockGridCoordinate(placePosition);
        var cgc = ChunkGridCoordinate.Parse(bgc);
        var rc = RelativeCoordinate.Parse(bgc);
        var block = chunkProvider.GetChunk(cgc).GetBlock(rc);

        Assert.AreEqual(BlockType.Grass, block.blockType);
    }

    [Test]
    public void BlockGridCoordinate_0_0_0にDirtを設置()
    {
        var chunkRepository = new OnMemoryChunkRepository();
        var chunkFactory = new AllDirtChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory, chunkRepository);
        var chunkBlockSetter = new ChunkBlockSetter(chunkRepository, chunkProvider);
        var placeBlockUseCase = new PlaceBlockUseCase(chunkBlockSetter);

        var placePosition = new Vector3(0, 0, 0);
        placeBlockUseCase.PlaceBlock(placePosition, BlockType.Dirt);

        var bgc = new BlockGridCoordinate(placePosition);
        var cgc = ChunkGridCoordinate.Parse(bgc);
        var rc = RelativeCoordinate.Parse(bgc);
        var block = chunkProvider.GetChunk(cgc).GetBlock(rc);

        Assert.AreEqual(BlockType.Dirt, block.blockType);
    }
}
