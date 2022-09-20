using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using BlockSystem;
using Cysharp.Threading.Tasks;
using Util;
using MasterData.Block;
using System;

public class BlockSystemTest
{
    [Test]
    public void チャンクメッシュ生成速度計測()
    {
        MasterBlockDataStore.InitialLoad();

        var mapGenerator = new MapGenerator(1024, 80);
        var chunkDataStore = new ChunkDataStore(mapGenerator);
        var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
        var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

        const int size = 4;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    var cc = new ChunkCoordinate(x, y, z);
                    var chunkData = chunkDataStore.GetChunkData(cc);
                    chunkMeshCreator.CreateMeshData(chunkData.Blocks);
                }
            }
        }
    }
}
