using System;
using System.Linq;
using System.Collections.Generic;
using DataObject.Block;
using Util;
using UniRx;

namespace BlockBehaviour
{
    internal class MUData
    {
        private Dictionary<BlockCoordinate, BlockData> blocks;
        internal BlockData[] Blocks => blocks.Values.ToArray();

        internal IObservable<Unit> OnBlockUpdate => _onBlockUpdate;
        private Subject<Unit> _onBlockUpdate = new Subject<Unit>();

        internal MUData(IEnumerable<BlockData> blocks)
        {
            this.blocks = blocks.ToDictionary(block => block.BlockCoordinate);

            foreach (var block in blocks)
            {
                UpdateBlockData(block);
            }
        }

        private void UpdateBlockData(BlockData blockData)
        {
            // 描画面計算
            var contactSurfaces = SurfaceNormal.Zero;
            var center = blockData.BlockCoordinate.ToInt3();

            foreach (var surface in SurfaceNormalExt.Array)
            {
                var aroundPosition = center + surface.ToInt3();
                if (!BlockCoordinate.IsValid(aroundPosition)) continue;

                var bc = new BlockCoordinate(aroundPosition);
                if (!blocks.TryGetValue(bc, out var aroundBlock)) continue;

                contactSurfaces = contactSurfaces.Add(surface);
            }

            blockData.SetContactOtherBlockSurfaces(contactSurfaces);
            blocks[blockData.BlockCoordinate] = blockData;

            _onBlockUpdate.OnNext(Unit.Default);
        }
    }
}
