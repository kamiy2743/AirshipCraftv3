using Util;
using MasterData.Block;
using System;

namespace BlockSystem
{
    /// <summary>
    /// ブロックの内部データ
    /// </summary>
    public struct BlockData : IEquatable<BlockData>
    {
        public readonly BlockID ID;
        public readonly BlockCoordinate BlockCoordinate;

        /// <summary>
        /// 他のブロックと接している面
        /// 直接アクセスは非推奨、<see cref="BlockData.IsContactOtherBlock(SurfaceNormal)"/>でアクセスしてください
        /// </summary>
        internal SurfaceNormal ContactOtherBlockSurfaces { get; private set; }

        /// <summary> 接している面を再計算する必要があるかどうか </summary>
        /// // アクセスの度に計算するのだと遅いから値変更時に確定させておく
        internal bool NeedToCalcContactSurfaces;

        /// <summary> 描画をスキップするかどうか </summary>
        // アクセスの度に計算するのだと遅いから値変更時に確定させておく
        internal bool IsRenderSkip;

        public static readonly BlockData Empty = new BlockData(BlockID.Empty, new BlockCoordinate(0, 0, 0));

        /// <summary>
        /// シリアライズ用なのでそれ以外では使用しないでください
        /// </summary>
        internal BlockData(BlockID id, BlockCoordinate bc, SurfaceNormal contactOtherBlockSurfaces)
        {
            ID = id;
            BlockCoordinate = bc;
            ContactOtherBlockSurfaces = contactOtherBlockSurfaces;
            NeedToCalcContactSurfaces = true;
            IsRenderSkip = false;
            SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);
        }

        internal BlockData(BlockID id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
            ContactOtherBlockSurfaces = SurfaceNormal.Empty;
            NeedToCalcContactSurfaces = true;
            IsRenderSkip = false;
            SetContactOtherBlockSurfaces(ContactOtherBlockSurfaces);
        }

        internal void SetContactOtherBlockSurfaces(SurfaceNormal surfaces)
        {
            ContactOtherBlockSurfaces = surfaces;
            // 接地面情報が空なら計算が必要
            NeedToCalcContactSurfaces = ContactOtherBlockSurfaces == SurfaceNormal.Empty;
            // 空気ブロックか、周りがすべてブロックで埋まっていれば描画しない
            IsRenderSkip = (ID == BlockID.Air) || ContactOtherBlockSurfaces.IsFull();
        }

        /// <summary>
        /// 指定された面で他のブロックと接しているかを返します
        /// </summary>
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
