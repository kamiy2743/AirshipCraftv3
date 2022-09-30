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

public class MessagePackTest
{
    [Test]
    public void ChunkDataのシリアライズ()
    {
        var mapGenerator = new MapGenerator(100, 100);
        var chunkData = new ChunkData(new ChunkCoordinate(0, 0, 0), mapGenerator);

        var sw1 = new System.Diagnostics.Stopwatch();
        sw1.Start();

        var bytes = MessagePackSerializer.Serialize(chunkData);
        UnityEngine.Debug.Log(bytes.Length);

        sw1.Stop();
        UnityEngine.Debug.Log(sw1.Elapsed);
    }
}
