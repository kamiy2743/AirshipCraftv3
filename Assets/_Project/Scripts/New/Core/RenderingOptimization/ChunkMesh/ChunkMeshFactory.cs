using System.Collections.Generic;
using UnityEngine;
using Domain;
using Domain.Chunks;
using RenderingOptimization.RenderingSurface;

namespace RenderingOptimization.ChunkMesh
{
    internal class ChunkMeshFactory
    {
        private IChunkProvider chunkProvider;
        private IChunkRenderingSurfaceProvider chunkRenderingSurfaceProvider;
        private IBlockMeshProvider blockMeshProvider;

        internal ChunkMeshFactory(IChunkProvider chunkProvider, IChunkRenderingSurfaceProvider chunkRenderingSurfaceProvider, IBlockMeshProvider blockMeshProvider)
        {
            this.chunkProvider = chunkProvider;
            this.chunkRenderingSurfaceProvider = chunkRenderingSurfaceProvider;
            this.blockMeshProvider = blockMeshProvider;
        }

        internal ChunkMesh Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var chunk = chunkProvider.GetChunk(chunkGridCoordinate);
            var chunkRenderingSurface = chunkRenderingSurfaceProvider.GetRenderingSurface(chunkGridCoordinate);

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);

                        var block = chunk.GetBlock(rc);
                        if (block.blockTypeID == BlockTypeID.Air)
                        {
                            continue;
                        }

                        var blockMesh = blockMeshProvider.GetBlockMesh(block.blockTypeID);
                        var blockRenderingSurface = chunkRenderingSurface.GetBlockRenderingSurface(rc);

                        if (!blockRenderingSurface.hasRenderingSurface)
                        {
                            continue;
                        }

                        foreach (var direction in DirectionExt.Array)
                        {
                            if (!blockRenderingSurface.Contains(direction))
                            {
                                continue;
                            }

                            var partMesh = blockMesh.GetPartMesh(direction);

                            foreach (var t in partMesh.triangles)
                            {
                                triangles.Add(vertices.Count + t);
                            }
                            foreach (var v in partMesh.vertices)
                            {
                                vertices.Add(v + (new Vector3(chunkGridCoordinate.x, chunkGridCoordinate.y, chunkGridCoordinate.z) * Chunk.BlockSide) + new Vector3(x, y, z));
                            }
                            uvs.AddRange(partMesh.uvs);
                        }

                        foreach (var t in blockMesh.otherPart.triangles)
                        {
                            triangles.Add(vertices.Count + t);
                        }
                        foreach (var v in blockMesh.otherPart.vertices)
                        {
                            vertices.Add(v + (new Vector3(chunkGridCoordinate.x, chunkGridCoordinate.y, chunkGridCoordinate.z) * Chunk.BlockSide) + new Vector3(x, y, z));
                        }
                        uvs.AddRange(blockMesh.otherPart.uvs);
                    }
                }
            }

            return new ChunkMesh(vertices.ToArray(), triangles.ToArray(), uvs.ToArray());
        }
    }
}