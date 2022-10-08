using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using BlockSystem;
using Cysharp.Threading.Tasks;
using Util;
using MasterData.Block;
using System;
using Unity.PerformanceTesting;

public class BlockSystemTest
{
    [Test, Performance]
    public void チャンクメッシュ生成速度計測_1チャンク()
    {
        MasterBlockDataStore.InitialLoad();

        Measure.Method(() =>
        {
            var mapGenerator = new MapGenerator(1024, 80);
            var chunkDataStore = new ChunkDataStore(mapGenerator);
            var chunkMeshCreator = new ChunkMeshCreator(chunkDataStore);

            var cc = new ChunkCoordinate(0, 0, 0);
            var chunkData = chunkDataStore.GetChunkData(cc, default);
            chunkMeshCreator.CreateMeshData(chunkData, default);
        })
        .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
        .IterationsPerMeasurement(10) // 計測一回辺りに走らせる処理の回数
        .MeasurementCount(20) // 計測数
        .Run();
    }

    [Test, Performance]
    public void チャンクメッシュ生成速度計測_64チャンク()
    {
        MasterBlockDataStore.InitialLoad();

        Measure.Method(() =>
        {
            var mapGenerator = new MapGenerator(1024, 80);
            var chunkDataStore = new ChunkDataStore(mapGenerator);
            var chunkMeshCreator = new ChunkMeshCreator(chunkDataStore);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int z = 0; z < 4; z++)
                    {
                        var cc = new ChunkCoordinate(x, y, z);
                        var chunkData = chunkDataStore.GetChunkData(cc, default);
                        chunkMeshCreator.CreateMeshData(chunkData, default);
                    }
                }
            }
        })
        .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
        .IterationsPerMeasurement(10) // 計測一回辺りに走らせる処理の回数
        .MeasurementCount(20) // 計測数
        .Run();
    }

    [Test]
    public void 生成済みのチャンクの再生成速度計測_1チャンク()
    {
        MasterBlockDataStore.InitialLoad();

        var mapGenerator = new MapGenerator(1024, 80);
        var chunkDataStore = new ChunkDataStore(mapGenerator);
        var chunkMeshCreator = new ChunkMeshCreator(chunkDataStore);

        var cc1 = new ChunkCoordinate(0, 0, 0);
        var chunkData1 = chunkDataStore.GetChunkData(cc1, default);
        chunkMeshCreator.CreateMeshData(chunkData1, default);

        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        var cc2 = new ChunkCoordinate(0, 0, 0);
        var chunkData2 = chunkDataStore.GetChunkData(cc2, default);
        chunkMeshCreator.CreateMeshData(chunkData2, default);

        sw.Stop();
        UnityEngine.Debug.Log(sw.Elapsed);
    }

    [Test, Performance]
    public void ChunkDataのコンストラクタ速度()
    {
        MasterBlockDataStore.InitialLoad();

        var mapGenerator = new MapGenerator(1024, 80);
        var cc = new ChunkCoordinate(0, 0, 0);

        Measure.Method(() =>
        {
            new ChunkData(cc, mapGenerator);
        })
        .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
        .IterationsPerMeasurement(10) // 計測一回辺りに走らせる処理の回数
        .MeasurementCount(20) // 計測数
        .Run();
    }
}
