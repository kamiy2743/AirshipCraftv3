using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using MasterData.Block;
using Util;

namespace BlockSystem.Serializer
{
    internal static class ChunkDataSerializer
    {
        private const int ChunkCoordinateByteSize = 6; //int16*3
        private const int BlockIDByteSize = 2; //uint16
        private const int LocalCoordinateByteSize = 3; //byte*3
        private const int SurfaceNormalByteSize = 1; //byte
        private const int BlockDataByteSize = BlockIDByteSize + LocalCoordinateByteSize + SurfaceNormalByteSize;
        internal const int ChunkDataByteSize = ChunkCoordinateByteSize + (BlockDataByteSize * ChunkData.BlockCountInChunk);

        internal static byte[] Serialize(ChunkData chunkData)
        {
            var result = new byte[ChunkDataByteSize];

            var cc = chunkData.ChunkCoordinate;
            result[0] = (byte)(cc.x >> 8);
            result[1] = (byte)cc.x;
            result[2] = (byte)(cc.y >> 8);
            result[3] = (byte)cc.y;
            result[4] = (byte)(cc.z >> 8);
            result[5] = (byte)cc.z;

            unsafe
            {
                fixed (global::BlockSystem.BlockData* blocksFirst = &chunkData.Blocks[0])
                fixed (byte* blocksResultFirst = &result[ChunkCoordinateByteSize])
                {
                    var job = new SerializeJob
                    {
                        blocksFirst = blocksFirst,
                        blocksResultFirst = blocksResultFirst
                    };

                    job.Schedule(ChunkData.BlockCountInChunk, 0).Complete();
                }
            }

            return result;
        }

        [BurstCompile]
        unsafe private struct SerializeJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public global::BlockSystem.BlockData* blocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public byte* blocksResultFirst;

            public void Execute(int index)
            {
                var blockData = blocksFirst + index;
                var offset = index * BlockDataByteSize;

                // BlockID
                *(blocksResultFirst + (offset++)) = (byte)((ushort)blockData->ID >> 8);
                *(blocksResultFirst + (offset++)) = (byte)(ushort)blockData->ID;

                // BlockCoordinateをLocalCoordinateに変換して書き込み
                var lc = global::BlockSystem.LocalCoordinate.FromBlockCoordinate(blockData->BlockCoordinate);
                *(blocksResultFirst + (offset++)) = lc.x;
                *(blocksResultFirst + (offset++)) = lc.y;
                *(blocksResultFirst + (offset++)) = lc.z;

                // ContactOtherBlockSurfaces
                *(blocksResultFirst + (offset++)) = (byte)blockData->ContactOtherBlockSurfaces;
            }
        }

        internal static ChunkData Deserialize(byte[] bytes)
        {
            var ccx = (bytes[0] << 8) + bytes[1];
            var ccy = (bytes[2] << 8) + bytes[3];
            var ccz = (bytes[4] << 8) + bytes[5];
            var cc = new ChunkCoordinate((short)ccx, (short)ccy, (short)ccz);

            var blocks = new BlockData[ChunkData.BlockCountInChunk];

            unsafe
            {
                fixed (global::BlockSystem.BlockData* blocksFirst = &blocks[0])
                fixed (byte* bytesFirst = &bytes[ChunkCoordinateByteSize])
                {
                    var job = new DeserializeJob
                    {
                        blocksFirst = blocksFirst,
                        bytesFirst = bytesFirst,
                        chunkRoot = new int3(cc.x, cc.y, cc.z) * ChunkData.ChunkBlockSide
                    };

                    job.Schedule(global::BlockSystem.ChunkData.BlockCountInChunk, 0).Complete();
                }
            }

            return new ChunkData(cc, blocks);
        }

        [BurstCompile]
        unsafe private struct DeserializeJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public global::BlockSystem.BlockData* blocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public byte* bytesFirst;

            [ReadOnly] public int3 chunkRoot;

            public void Execute(int index)
            {
                var offset = index * BlockDataByteSize;

                // BlockID
                var blockID = (BlockID)(*(bytesFirst + (offset++)) << 8) + *(bytesFirst + (offset++));

                // BlockCoordinate
                var lcx = *(bytesFirst + (offset++));
                var lcy = *(bytesFirst + (offset++));
                var lcz = *(bytesFirst + (offset++));
                var bc = new BlockCoordinate(
                    chunkRoot.x + lcx,
                    chunkRoot.y + lcy,
                    chunkRoot.z + lcz
                );

                // ContactOtherBlockSurfaces
                var contactOtherBlockSurfaces = (SurfaceNormal)(*(bytesFirst + offset));

                *(blocksFirst + index) = new BlockData(blockID, bc, contactOtherBlockSurfaces);
            }
        }
    }
}