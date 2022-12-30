using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
// using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using Util;
using MasterData.Block;
using System;
using UnityEngine;
using Unity.PerformanceTesting;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DataObject.Block;
using DataObject.Chunk;
using Unity.Mathematics;

public class Test
{
    [Test]
    public void ChunkDataの最大容量()
    {
        UnityEngine.Debug.Log(long.MaxValue / Math.Pow(ushort.MaxValue + 1, 3));
    }

    [Test, Performance]
    public void Hash計算の速度計測()
    {
        Measure.Method(() =>
        {
            for (int i = 0; i < 256; i++)
            {
                math.hash(new int3(i, i, i));
            }
        })
        .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
        .IterationsPerMeasurement(10000) // 計測一回辺りに走らせる処理の回数
        .MeasurementCount(20) // 計測数
        .Run();

        Measure.Method(() =>
        {
            for (int i = 0; i < 256; i++)
            {
                HashCode.Combine(i, i, i);
            }
        })
        .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
        .IterationsPerMeasurement(10000) // 計測一回辺りに走らせる処理の回数
        .MeasurementCount(20) // 計測数
        .Run();
    }
}
