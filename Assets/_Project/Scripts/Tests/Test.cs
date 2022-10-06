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

public class Test
{
    [Test]
    public void ChunkDataの最大容量()
    {
        UnityEngine.Debug.Log(long.MaxValue / Math.Pow(ushort.MaxValue + 1, 3));
    }
}
