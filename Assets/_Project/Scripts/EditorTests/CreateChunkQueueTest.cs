using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using UnityView.Rendering.Chunks;
using Domain;
using Unity.Mathematics;

public class CreateChunkQueueTest
{
    [Test]
    public void 動作確認()
    {
        var radius = 3;
        var side = radius * 2 + 1;
        var createChunkQueue = new CreateChunkQueue(side * side * side);
        var playerChunk = new ChunkGridCoordinate(0, 0, 0);

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                if (!playerChunk.TryAdd(new int3(x, 0, z), out var cgc))
                {
                    continue;
                }

                var distance = (x * x) + (z * z);
                createChunkQueue.Enqueue(distance, cgc);
            }
        }

        var grid = new int[side, side];

        var length = createChunkQueue.Count;
        for (int i = 0; i < length; i++)
        {
            createChunkQueue.TryDequeue(out var cgc);
            grid[cgc.X + radius, cgc.Z + radius] = i;
        }

        for (int z = side - 1; z >= 0; z--)
        {
            var line = "";
            for (int x = 0; x < side; x++)
            {
                line += grid[x, z].ToString("D2") + ", ";
            }
            UnityEngine.Debug.Log(line + "\n");
        }
    }
}
