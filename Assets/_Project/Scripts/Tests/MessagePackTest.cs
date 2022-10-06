using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
// using UnityEngine.TestTools;
using BlockSystem;
using Cysharp.Threading.Tasks;
using Util;
using MasterData.Block;
using System;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using System.IO;
using System.Text;
using Unity.PerformanceTesting;
using System.Threading;

public class MessagePackTest
{
    [Test]
    public void ChunkDataのシリアライズ()
    {
        var mapGenerator = new MapGenerator(100, 100);
        var chunkData = new ChunkData(new ChunkCoordinate(12, 2, 35), mapGenerator);

        var sw1 = new System.Diagnostics.Stopwatch();
        sw1.Start();

        var bytes = MessagePackSerializer.Serialize(chunkData);
        var chunkData2 = MessagePackSerializer.Deserialize<ChunkData>(bytes);
        UnityEngine.Debug.Log(chunkData2.ChunkCoordinate);
        UnityEngine.Debug.Log(MessagePackSerializer.ConvertToJson(bytes));

        sw1.Stop();
        UnityEngine.Debug.Log(sw1.Elapsed);
    }

    [Test]
    public void ChunkCoordinateのシリアライズ()
    {
        var bytes = MessagePackSerializer.Serialize(new ChunkCoordinate(124, 1, 11));
        var cc = MessagePackSerializer.Deserialize<ChunkCoordinate>(bytes);
        UnityEngine.Debug.Log(cc);
    }

    [Test]
    public void Structのシリアライズ()
    {
        var bytes = MessagePackSerializer.Serialize(new Vector3(213, 1231, 321));
        var vector = MessagePackSerializer.Deserialize<Vector3>(bytes);
        UnityEngine.Debug.Log(vector);
    }

    [Test, Performance]
    public void SerializeとDeserializeの速度計測()
    {
        var cts = new CancellationTokenSource();
        var path = Application.persistentDataPath + "/FileStreamTest";
        File.Create(path).Close();

        try
        {
            Measure.Method(() =>
            {
                MessagePackSerializer.Serialize(ChunkData.Empty, cancellationToken: cts.Token);
            })
            .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
            .IterationsPerMeasurement(10) // 計測一回辺りに走らせる処理の回数
            .MeasurementCount(20) // 計測数
            .Run();

            var bytes = MessagePackSerializer.Serialize(ChunkData.Empty, cancellationToken: cts.Token);
            Measure.Method(() =>
            {
                MessagePackSerializer.Deserialize<ChunkData>(bytes, cancellationToken: cts.Token);
            })
            .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
            .IterationsPerMeasurement(10) // 計測一回辺りに走らせる処理の回数
            .MeasurementCount(20) // 計測数
            .Run();

        }
        finally
        {
            cts.Cancel();
            cts.Dispose();
            File.Delete(path);
        }

    }
}
