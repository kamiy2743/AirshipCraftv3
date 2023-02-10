using System.Collections.Generic;
using UnityEngine;
using Domain;
using Domain.Chunks;

namespace UnityView.Rendering.Chunks
{
    class ChunkMeshFactory
    {
        readonly IChunkProvider _chunkProvider;
        readonly ChunkSurfaceProvider _chunkSurfaceProvider;
        readonly BlockMeshProvider _blockMeshProvider;

        internal ChunkMeshFactory(IChunkProvider chunkProvider, ChunkSurfaceProvider chunkSurfaceProvider, BlockMeshProvider blockMeshProvider)
        {
            _chunkProvider = chunkProvider;
            _chunkSurfaceProvider = chunkSurfaceProvider;
            _blockMeshProvider = blockMeshProvider;
        }

        internal ChunkMesh Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var chunk = _chunkProvider.GetChunk(chunkGridCoordinate);
            var chunkSurface = _chunkSurfaceProvider.GetChunkSurface(chunkGridCoordinate);

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

                        var blockSurface = chunkSurface.GetBlockSurface(rc);
                        if (!blockSurface.HasRenderingSurface)
                        {
                            continue;
                        }

                        var blockType = chunk.GetBlock(rc).BlockType;
                        var blockMesh = _blockMeshProvider.GetBlockMesh(blockType);

                        foreach (var face in FaceExt.Array)
                        {
                            if (!blockSurface.Contains(face))
                            {
                                continue;
                            }

                            var faceMesh = blockMesh.GetFaceMesh(face);

                            foreach (var t in faceMesh.Triangles)
                            {
                                triangles.Add(vertices.Count + t);
                            }
                            foreach (var v in faceMesh.Vertices)
                            {
                                vertices.Add(v + new Vector3(x, y, z));
                            }
                            uvs.AddRange(faceMesh.Uvs);
                        }

                        foreach (var t in blockMesh.OtherPart.Triangles)
                        {
                            triangles.Add(vertices.Count + t);
                        }
                        foreach (var v in blockMesh.OtherPart.Vertices)
                        {
                            vertices.Add(v + new Vector3(x, y, z));
                        }
                        uvs.AddRange(blockMesh.OtherPart.Uvs);
                    }
                }
            }

            return new ChunkMesh(chunkGridCoordinate, vertices.ToArray(), triangles.ToArray(), uvs.ToArray());
        }
    }
}