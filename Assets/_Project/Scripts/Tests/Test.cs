using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using BlockSystem;
using Cysharp.Threading.Tasks;
using Util;

public class Test
{
    [UnityTest]
    public IEnumerator チャンク生成のシステム部分の実行速度計測()
    {
        var chunkDataStore = new ChunkDataStore();
        var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
        var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

        for (int x = 0; x < WorldSettings.WorldChunkSideXZ; x++)
        {
            for (int y = 0; y < WorldSettings.WorldChunkSideY; y++)
            {
                for (int z = 0; z < WorldSettings.WorldChunkSideXZ; z++)
                {
                    chunkMeshCreator.CreateMesh(chunkDataStore.GetChunkData(new ChunkCoordinate(x, y, z)));
                    yield return UniTask.DelayFrame(1).ToCoroutine();
                }
            }
        }
    }

    [Test]
    public void 割り算速度計測()
    {
        int a = 164;
        int b = 16;

        var sw1 = new System.Diagnostics.Stopwatch();
        sw1.Start();
        for (int i = 0; i < 10000; i++)
        {
            var result = a / b;
        }
        sw1.Stop();
        Debug.Log($"{a / b}: " + sw1.Elapsed);

        var sw2 = new System.Diagnostics.Stopwatch();
        sw2.Start();
        float c = 1f / (float)b;
        for (int i = 0; i < 10000; i++)
        {
            var result = a * c;
        }
        sw2.Stop();
        Debug.Log($"{(int)(a * c)}: " + sw2.Elapsed);
    }

    [Test]
    public void mod速度計測()
    {
        int a = 164;
        int b = 16;

        var sw1 = new System.Diagnostics.Stopwatch();
        sw1.Start();
        for (int i = 0; i < 10000; i++)
        {
            var result = a % b;
        }
        sw1.Stop();
        Debug.Log($"a % b: " + sw1.Elapsed);
        Debug.Log(a % b);

        var sw3 = new System.Diagnostics.Stopwatch();
        sw3.Start();
        float c = 1f / (float)b;
        for (int i = 0; i < 10000; i++)
        {
            var result = a - ((int)(a * c) * b);
        }
        sw3.Stop();
        Debug.Log($"a - (int)(a * c): " + sw3.Elapsed);
        Debug.Log(a - ((int)(a * c) * b));
    }

    [Test]
    public void EnumGetValuesの速度計測()
    {
        var sw1 = new System.Diagnostics.Stopwatch();
        sw1.Start();
        foreach (SurfaceNormal surface in System.Enum.GetValues(typeof(SurfaceNormal)))
        {
            // Debug.Log(surface);
        }
        sw1.Stop();
        Debug.Log(sw1.Elapsed);

        var sw2 = new System.Diagnostics.Stopwatch();
        sw2.Start();
        foreach (var surface in SurfaceNormalExt.List)
        {
            // Debug.Log(surface);
        }
        sw2.Stop();
        Debug.Log(sw2.Elapsed);
    }
}