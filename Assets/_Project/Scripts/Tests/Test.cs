using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
// using UnityEngine.TestTools;
using BlockSystem;
using Cysharp.Threading.Tasks;
using Util;
using MasterData.Block;
using System;
using UnityEngine;
using Unity.PerformanceTesting;
using BlockSystem.Serializer;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DataObject.Block;
using DataObject.Chunk;

public class Test
{
    [Test]
    public void ChunkDataの最大容量()
    {
        UnityEngine.Debug.Log(long.MaxValue / Math.Pow(ushort.MaxValue + 1, 3));
    }

    [Test, Performance]
    public void AAA()
    {
        var chunkData = ChunkData.Empty;

        Measure.Method(() =>
        {
            ChunkDataSerializer.Serialize(chunkData);
        })
        .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
        .IterationsPerMeasurement(1000) // 計測一回辺りに走らせる処理の回数
        .MeasurementCount(20) // 計測数
        .Run();

        unsafe
        {
            var bytes = new byte[6 * ChunkData.BlockCountInChunk];

            Measure.Method(() =>
            {
                fixed (void* rbp = &bytes[0], bp = &chunkData.Blocks[0])
                {
                    var resultOffset = 0;
                    var resultBytesPtr = (byte*)rbp;
                    var blocksPtr = (byte*)bp;
                    for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
                    {
                        var blocksOffset = i * 24;
                        *(resultBytesPtr + resultOffset++) = *(blocksPtr + blocksOffset + 2);
                        *(resultBytesPtr + resultOffset++) = *(blocksPtr + blocksOffset + 3);
                        for (int j = 0; j < 4; j++)
                        {
                            *(resultBytesPtr + resultOffset++) = *(blocksPtr + blocksOffset + 7 + j);
                        }
                    }
                }
            })
            .WarmupCount(5) // 記録する前に何回か処理を走らせる（安定性を向上させるため）
            .IterationsPerMeasurement(1000) // 計測一回辺りに走らせる処理の回数
            .MeasurementCount(20) // 計測数
            .Run();
        }
    }

    [BurstCompile]
    private unsafe struct Job : IJob
    {
        [NativeDisableUnsafePtrRestriction][ReadOnly] public byte* blocksPtr;
        [NativeDisableUnsafePtrRestriction][ReadOnly] public byte* resultBytesPtr;
        [ReadOnly] public int blockDataByteSize;

        public void Execute()
        {
            var resultOffset = 0;
            for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
            {
                var blocksOffset = i * blockDataByteSize;
                *(resultBytesPtr + resultOffset++) = *(blocksPtr + blocksOffset + 2);
                *(resultBytesPtr + resultOffset++) = *(blocksPtr + blocksOffset + 3);
                for (int j = 0; j < 4; j++)
                {
                    *(resultBytesPtr + resultOffset++) = *(blocksPtr + blocksOffset + 7 + j);
                }
            }
        }
    }
}
