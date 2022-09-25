// using System.Collections;
// using System.Collections.Generic;

// using NUnit.Framework;
// // using UnityEngine.TestTools;
// using BlockSystem;
// using Cysharp.Threading.Tasks;
// using Util;
// using MasterData.Block;
// using System;

// public class BlockSystemTest
// {
//     [Test]
//     public void チャンクメッシュ生成速度計測_1チャンク()
//     {
//         MasterBlockDataStore.InitialLoad();

//         var mapGenerator = new MapGenerator(1024, 80);
//         var chunkDataStore = new ChunkDataStore(mapGenerator);
//         var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
//         var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

//         var sw = new System.Diagnostics.Stopwatch();
//         sw.Start();

//         var cc = new ChunkCoordinate(0, 0, 0);
//         var chunkData = chunkDataStore.GetChunkData(cc);
//         chunkMeshCreator.CreateMeshData(ref chunkData.Blocks);

//         sw.Stop();
//         UnityEngine.Debug.Log(sw.Elapsed);
//     }

//     [Test]
//     public void チャンクメッシュ生成速度計測_64チャンク()
//     {
//         MasterBlockDataStore.InitialLoad();

//         var mapGenerator = new MapGenerator(1024, 80);
//         var chunkDataStore = new ChunkDataStore(mapGenerator);
//         var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
//         var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

//         var sw = new System.Diagnostics.Stopwatch();
//         sw.Start();

//         for (int x = 0; x < 4; x++)
//         {
//             for (int y = 0; y < 4; y++)
//             {
//                 for (int z = 0; z < 4; z++)
//                 {
//                     var cc = new ChunkCoordinate(x, y, z);
//                     var chunkData = chunkDataStore.GetChunkData(cc);
//                     chunkMeshCreator.CreateMeshData(ref chunkData.Blocks);
//                 }
//             }
//         }

//         sw.Stop();
//         UnityEngine.Debug.Log(sw.Elapsed);
//     }

//     [Test]
//     public void チャンクメッシュ生成速度計測_64チャンク_meshData使いまわし()
//     {
//         MasterBlockDataStore.InitialLoad();

//         var mapGenerator = new MapGenerator(1024, 80);
//         var chunkDataStore = new ChunkDataStore(mapGenerator);
//         var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
//         var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

//         var sw = new System.Diagnostics.Stopwatch();
//         sw.Start();

//         ChunkMeshData meshData = null;
//         for (int x = 0; x < 4; x++)
//         {
//             for (int y = 0; y < 4; y++)
//             {
//                 for (int z = 0; z < 4; z++)
//                 {
//                     var cc = new ChunkCoordinate(x, y, z);
//                     var chunkData = chunkDataStore.GetChunkData(cc);
//                     meshData = chunkMeshCreator.CreateMeshData(ref chunkData.Blocks, meshData);
//                     meshData.Clear();
//                 }
//             }
//         }

//         sw.Stop();
//         UnityEngine.Debug.Log(sw.Elapsed);
//     }

//     [Test]
//     public void 生成済みのチャンクの再生成速度計測_1チャンク()
//     {
//         MasterBlockDataStore.InitialLoad();

//         var mapGenerator = new MapGenerator(1024, 80);
//         var chunkDataStore = new ChunkDataStore(mapGenerator);
//         var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
//         var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

//         var cc1 = new ChunkCoordinate(0, 0, 0);
//         var chunkData1 = chunkDataStore.GetChunkData(cc1);
//         chunkMeshCreator.CreateMeshData(ref chunkData1.Blocks);

//         var sw = new System.Diagnostics.Stopwatch();
//         sw.Start();

//         var cc2 = new ChunkCoordinate(0, 0, 0);
//         var chunkData2 = chunkDataStore.GetChunkData(cc2);
//         chunkMeshCreator.CreateMeshData(ref chunkData2.Blocks);

//         sw.Stop();
//         UnityEngine.Debug.Log(sw.Elapsed);
//     }
// }
