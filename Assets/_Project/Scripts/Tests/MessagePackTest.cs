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
