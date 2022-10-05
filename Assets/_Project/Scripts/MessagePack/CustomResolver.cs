#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

using MessagePack;
using MessagePack.Formatters;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace MyMessagePackExt.Resolvers
{
    public class CustomResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new CustomResolver();

        private CustomResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = CustomResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class CustomResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, int> lookup;

        static CustomResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(1)
            {
                { typeof(global::BlockSystem.BlockCoordinate), 0 },
                { typeof(global::BlockSystem.ChunkCoordinate), 1 },
                { typeof(global::BlockSystem.ChunkData), 2 },
                { typeof(global::MasterData.Block.BlockID), 3 },
                { typeof(global::Util.SurfaceNormal), 4 },
                { typeof(global::BlockSystem.ChunkDataIndex), 5 },
            };
        }

        internal static object GetFormatter(global::System.Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new MyMessagePackExt.Formatters.BlockSystem.CustomBlockCoordinateFormatter();
                case 1: return new MyMessagePackExt.Formatters.BlockSystem.CustomChunkCoordinateFormatter();
                case 2: return new MyMessagePackExt.Formatters.BlockSystem.CustomChunkDataFormatter();
                case 3: return new MyMessagePackExt.Formatters.MasterData.Block.CustomBlockIDFormatter();
                case 4: return new MyMessagePackExt.Formatters.Util.CustomSurfaceNormalFormatter();
                case 5: return new MyMessagePackExt.Formatters.BlockSystem.CustomChunkDataIndexFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1649 // File name should match first type name


// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MyMessagePackExt.Formatters.MasterData.Block
{

    public sealed class CustomBlockIDFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MasterData.Block.BlockID>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::MasterData.Block.BlockID value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.UInt16)value);
        }

        public global::MasterData.Block.BlockID Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::MasterData.Block.BlockID)reader.ReadUInt16();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name

// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MyMessagePackExt.Formatters.Util
{

    public sealed class CustomSurfaceNormalFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Util.SurfaceNormal>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Util.SurfaceNormal value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((byte)value);
        }

        public global::Util.SurfaceNormal Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Util.SurfaceNormal)reader.ReadByte();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name


// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MyMessagePackExt.Formatters.BlockSystem
{
    public sealed class CustomBlockCoordinateFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::BlockSystem.BlockCoordinate>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::BlockSystem.BlockCoordinate value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(3);
            writer.WriteUInt32(value.x);
            writer.WriteUInt32(value.y);
            writer.WriteUInt32(value.z);
        }

        public global::BlockSystem.BlockCoordinate Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return MyMessagePackExt.Resolvers.GeneratedResolver.Instance.GetFormatter<global::BlockSystem.BlockCoordinate>().Deserialize(ref reader, options);
        }
    }

    public sealed class CustomChunkCoordinateFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::BlockSystem.ChunkCoordinate>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::BlockSystem.ChunkCoordinate value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(3);
            writer.WriteUInt16(value.x);
            writer.WriteUInt16(value.y);
            writer.WriteUInt16(value.z);
        }

        public global::BlockSystem.ChunkCoordinate Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return MyMessagePackExt.Resolvers.GeneratedResolver.Instance.GetFormatter<global::BlockSystem.ChunkCoordinate>().Deserialize(ref reader, options);
        }
    }

    public sealed class CustomChunkDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::BlockSystem.ChunkData>
    {

        private CustomBlockDataArrayFormatter customBlockDataArrayFormatter = new CustomBlockDataArrayFormatter();

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::BlockSystem.ChunkData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::BlockSystem.ChunkCoordinate>(formatterResolver).Serialize(ref writer, value.ChunkCoordinate, options);
            customBlockDataArrayFormatter.Serialize(ref writer, value.Blocks, options);
        }

        public global::BlockSystem.ChunkData Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __ChunkCoordinate__ = default(global::BlockSystem.ChunkCoordinate);
            var __Blocks__ = default(global::BlockSystem.BlockData[]);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __ChunkCoordinate__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::BlockSystem.ChunkCoordinate>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        __Blocks__ = customBlockDataArrayFormatter.Deserialize(ref reader, __ChunkCoordinate__, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::BlockSystem.ChunkData(__ChunkCoordinate__, __Blocks__);
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class CustomBlockDataArrayFormatter : IMessagePackFormatter<global::BlockSystem.BlockData[]>
    {
        // uint16
        private const int BlockIDByteSize = 2;
        // byte * 3
        private const int LocalCoordinateByteSize = 3;
        // byte
        private const int SurfaceNormalByteSize = 1;

        private const int BlockDataByteSize = BlockIDByteSize + LocalCoordinateByteSize + SurfaceNormalByteSize;

        public void Serialize(ref MessagePackWriter writer, global::BlockSystem.BlockData[] value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                var job = new SerializeJob();
                job.blockDataByteSize = BlockDataByteSize;

                var result = new byte[BlockDataByteSize * value.Length];
                unsafe
                {
                    fixed (global::BlockSystem.BlockData* blocksFirst = &value[0])
                    fixed (byte* resultFirst = &result[0])
                    {
                        job.blocksFirst = blocksFirst;
                        job.resultFirst = resultFirst;
                        job.Schedule(value.Length, 0).Complete();
                    }
                }

                writer.CancellationToken.ThrowIfCancellationRequested();
                writer.WriteArrayHeader(value.Length);
                writer.Write(result);
            }
        }

        [BurstCompile]
        unsafe private struct SerializeJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public global::BlockSystem.BlockData* blocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public byte* resultFirst;

            [ReadOnly]
            public int blockDataByteSize;

            public void Execute(int index)
            {
                var blockData = blocksFirst + index;
                var offset = index * blockDataByteSize;

                // BlockID
                *(resultFirst + (offset++)) = (byte)((ushort)blockData->ID >> 8);
                *(resultFirst + (offset++)) = (byte)(ushort)blockData->ID;

                // BlockCoordinateをLocalCoordinateに変換して書き込み
                var lc = global::BlockSystem.LocalCoordinate.FromBlockCoordinate(blockData->BlockCoordinate);
                *(resultFirst + (offset++)) = lc.x;
                *(resultFirst + (offset++)) = lc.y;
                *(resultFirst + (offset++)) = lc.z;

                // ContactOtherBlockSurfaces
                *(resultFirst + (offset++)) = (byte)blockData->ContactOtherBlockSurfaces;
            }
        }

        private global::BlockSystem.ChunkCoordinate chunkCoordinate;
        public global::BlockSystem.BlockData[] Deserialize(ref MessagePackReader reader, global::BlockSystem.ChunkCoordinate chunkCoordinate, MessagePackSerializerOptions options)
        {
            this.chunkCoordinate = chunkCoordinate;
            return Deserialize(ref reader, options);
        }

        public global::BlockSystem.BlockData[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }

            var len = reader.ReadArrayHeader();
            if (len == 0)
            {
                return System.Array.Empty<global::BlockSystem.BlockData>();
            }

            var array = new global::BlockSystem.BlockData[len];

            // byte[]書き込み時のヘッダ部分以外を取得
            var bytes = System.Buffers.BuffersExtensions.ToArray(reader.ReadRaw().Slice(3));

            var job = new DeserializeJob();
            job.blockDataByteSize = BlockDataByteSize;
            job.chunkCoordinate = chunkCoordinate;

            unsafe
            {
                fixed (global::BlockSystem.BlockData* blocksFirst = &array[0])
                fixed (byte* bytesFirst = &bytes[0])
                {
                    job.blocksFirst = blocksFirst;
                    job.bytesFirst = bytesFirst;
                    job.Schedule(len, 0).Complete();
                }
            }

            return array;
        }

        [BurstCompile]
        unsafe private struct DeserializeJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public global::BlockSystem.BlockData* blocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public byte* bytesFirst;

            [ReadOnly]
            public int blockDataByteSize;

            [ReadOnly]
            public global::BlockSystem.ChunkCoordinate chunkCoordinate;

            public void Execute(int index)
            {
                var offset = index * blockDataByteSize;

                // BlockID
                var blockID = (global::MasterData.Block.BlockID)((*(bytesFirst + (offset++)) << 8) + *(bytesFirst + (offset++)));

                // BlockCoordinate
                var lcx = *(bytesFirst + (offset++));
                var lcy = *(bytesFirst + (offset++));
                var lcz = *(bytesFirst + (offset++));
                var lc = new global::BlockSystem.LocalCoordinate(lcx, lcy, lcz);
                var bc = global::BlockSystem.BlockCoordinate.FromChunkAndLocal(chunkCoordinate, lc);

                // ContactOtherBlockSurfaces
                var contactOtherBlockSurfaces = (global::Util.SurfaceNormal)(*(bytesFirst + offset));

                *(blocksFirst + index) = new global::BlockSystem.BlockData(blockID, bc, contactOtherBlockSurfaces);
            }
        }
    }

    public sealed class CustomChunkDataIndexFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::BlockSystem.ChunkDataIndex>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::BlockSystem.ChunkDataIndex value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::BlockSystem.ChunkCoordinate>(formatterResolver).Serialize(ref writer, value.ChunkCoordinate, options);
            writer.WriteInt64(value.Index);
        }

        public global::BlockSystem.ChunkDataIndex Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return MyMessagePackExt.Resolvers.GeneratedResolver.Instance.GetFormatter<global::BlockSystem.ChunkDataIndex>().Deserialize(ref reader, options);
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
