using ACv3.Domain.Chunks;
using Unity.Mathematics;
using UnityEngine;

namespace ACv3.Presentation.Debug
{
    class ChunkDebugger : MonoBehaviour
    {
        [SerializeField] Transform player;
        [Space(20)]
        [SerializeField] bool drawGizmo = true;
        [SerializeField, Range(0, 16)] int drawRadius;
        [SerializeField] bool simpleDraw = false;

        void OnDrawGizmos()
        {
            if (!drawGizmo) return;
            DrawLoadedChunk();
        }

        void DrawLoadedChunk()
        {
            var pc = GetPlayerChunk(player.position);
            var size = Vector3.one * Chunk.BlockSide;

            if (simpleDraw)
            {
                Gizmos.color = GetRadiusColor(drawRadius);
                Gizmos.DrawWireCube(pc * Chunk.BlockSide + size * 0.5f, Vector3.one * (drawRadius * 2 + 1) * Chunk.BlockSide);
                Gizmos.DrawWireCube(pc * Chunk.BlockSide + size * 0.5f, Vector3.one * ((drawRadius - 1) * 2 + 1) * Chunk.BlockSide);
                return;
            }

            void DrawWireCube(Vector3 chunkDelta = default, int radius = 0)
            {
                Gizmos.color = GetRadiusColor(radius);
                Gizmos.DrawWireCube((pc + chunkDelta) * Chunk.BlockSide + (size * 0.5f), size);
            }

            for (int r = drawRadius; r > 0; r--)
            {
                for (int x = -r; x <= r; x++)
                {
                    for (int y = -r; y <= r; y++)
                    {
                        for (int z = -r; z <= r; z++)
                        {
                            DrawWireCube(new Vector3(x, y, z), r);
                        }
                    }
                }
            }

            DrawWireCube();
        }

        Vector3 GetPlayerChunk(Vector3 playerPosition)
        {
            return new Vector3(
                (int)math.floor(playerPosition.x) >> Chunk.BlockSideShift,
                (int)math.floor(playerPosition.y) >> Chunk.BlockSideShift,
                (int)math.floor(playerPosition.z) >> Chunk.BlockSideShift)
           ;
        }

        Color GetRadiusColor(int radius)
        {
            var h = (1f / 8f) * Mathf.Repeat(radius, 8);
            return Color.HSVToRGB(h, 1, 1);
        }
    }
}
