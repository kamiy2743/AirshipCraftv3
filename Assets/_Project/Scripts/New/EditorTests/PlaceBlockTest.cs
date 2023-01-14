using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Domain.Chunks;
using Infrastructure;
using UseCase;

public class PlaceBlockTest
{
    [Test]
    public void BlockGridCoordinate_0_0_0に設置すると4つのチャンクを更新する()
    {
        var chunkRepository = new OnMemoryChunkRepository();
        var chunkFactory = new ChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory);
        var setBlockService = new SetBlockService(chunkRepository, chunkProvider);
        var placeBlockUseCase = new PlaceBlockUseCase(setBlockService);

        placeBlockUseCase.PlaceBlock(new Vector3(0, 0, 0));

        Assert.AreEqual(4, chunkRepository.chunks.Values.Count);
    }

    [Test]
    public void BlockGridCoordinate_8_8_8に設置すると1つのチャンクを更新する()
    {
        var chunkRepository = new OnMemoryChunkRepository();
        var chunkFactory = new ChunkFactory();
        var chunkProvider = new ChunkProvider(chunkFactory);
        var setBlockService = new SetBlockService(chunkRepository, chunkProvider);
        var placeBlockUseCase = new PlaceBlockUseCase(setBlockService);

        placeBlockUseCase.PlaceBlock(new Vector3(8, 8, 8));

        Assert.AreEqual(1, chunkRepository.chunks.Values.Count);
    }
}
