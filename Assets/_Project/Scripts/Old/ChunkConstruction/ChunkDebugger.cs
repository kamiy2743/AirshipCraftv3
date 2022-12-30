using UnityEngine;
using Unity.Mathematics;
using DataObject.Chunk;

namespace ChunkConstruction
{
    public class ChunkDebugger : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [Space(20)]
        [SerializeField] private bool drawGizmo = true;
        [SerializeField, Range(0, 16)] private int drawRadius;
        [SerializeField] private bool simpleDraw = false;

        private void OnDrawGizmos()
        {
            if (!drawGizmo) return;
            DrawLoadedChunk();
        }

        private void DrawLoadedChunk()
        {
            var pc = GetPlayerChunk(player.position);
            var size = Vector3.one * ChunkData.ChunkBlockSide;

            if (simpleDraw)
            {
                Gizmos.color = GetRadiusColor(drawRadius);
                Gizmos.DrawWireCube(pc * ChunkData.ChunkBlockSide + size * 0.5f, Vector3.one * (drawRadius * 2 + 1) * ChunkData.ChunkBlockSide);
                Gizmos.DrawWireCube(pc * ChunkData.ChunkBlockSide + size * 0.5f, Vector3.one * ((drawRadius - 1) * 2 + 1) * ChunkData.ChunkBlockSide);
                return;
            }

            void DrawWireCube(Vector3 chunkDelta = default, int radius = 0)
            {
                Gizmos.color = GetRadiusColor(radius);
                Gizmos.DrawWireCube((pc + chunkDelta) * ChunkData.ChunkBlockSide + (size * 0.5f), size);
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

        private Vector3 GetPlayerChunk(Vector3 playerPosition)
        {
            return new Vector3(
                (int)math.floor(playerPosition.x) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.y) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.z) >> ChunkData.ChunkBlockSideShift)
           ;
        }

        private Color GetRadiusColor(int radius)
        {
            var h = (1f / 8f) * Mathf.Repeat(radius, 8);
            return Color.HSVToRGB(h, 1, 1);
        }
    }
}
