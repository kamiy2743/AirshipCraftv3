using Util;
using MasterData.Block;
using System;
using MessagePack;

namespace BlockSystem
{
    [MessagePackObject]
    public struct BlockData : IEquatable<BlockData>
    {
        [Key(0)]
        public readonly BlockID ID;
        [Key(1)]
        public readonly BlockCoordinate BlockCoordinate;

        [IgnoreMember]
        public static readonly BlockData Empty = new BlockData(BlockID.Empty, new BlockCoordinate(0, 0, 0));

        [Key(2)]
        public SurfaceNormal ContactOtherBlockSurfaces { get; private set; }

        internal bool NeedToCalcContactSurfaces => ContactOtherBlockSurfaces == SurfaceNormal.Empty;

        // アクセスの度に計算するのだと遅いから値変更時に確定させておく
        internal bool IsRenderSkip;

        [SerializationConstructor]
        public BlockData(BlockID id, BlockCoordinate bc, SurfaceNormal contactOtherBlockSurfaces)
        {
            ID = id;
            BlockCoordinate = bc;
            ContactOtherBlockSurfaces = contactOtherBlockSurfaces;
            IsRenderSkip = false;
            SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);
        }

        internal BlockData(BlockID id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
            ContactOtherBlockSurfaces = SurfaceNormal.Empty;
            IsRenderSkip = false;
            SetContactOtherBlockSurfaces(ContactOtherBlockSurfaces);
        }

        internal void SetContactOtherBlockSurfaces(SurfaceNormal surfaces)
        {
            ContactOtherBlockSurfaces = surfaces;
            IsRenderSkip = ID == BlockID.Air || ContactOtherBlockSurfaces.IsFull();
        }

        internal bool IsContactOtherBlock(SurfaceNormal surface)
        {
            return ContactOtherBlockSurfaces.Contains(surface);
        }

        public override bool Equals(object obj)
        {
            return obj is BlockData data && Equals(data);
        }

        public bool Equals(BlockData other)
        {
            if (this.ID != other.ID) return false;
            if (this.BlockCoordinate != other.BlockCoordinate) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)ID, BlockCoordinate.x, BlockCoordinate.y, BlockCoordinate.z);
        }

        public static bool operator ==(BlockData left, BlockData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockData left, BlockData right)
        {
            return !(left == right);
        }
    }
}
