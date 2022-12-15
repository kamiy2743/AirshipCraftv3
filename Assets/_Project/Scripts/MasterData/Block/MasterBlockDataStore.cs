using System.Linq;
using System.Collections.Generic;
using DataObject.Block;

namespace MasterData.Block
{
    public class MasterBlockDataStore
    {
        private Dictionary<BlockID, MasterBlockData> masterBlockDataDictionary = new Dictionary<BlockID, MasterBlockData>();
        public IReadOnlyList<MasterBlockData> MasterBlockDataList;
        public readonly int BlocksCount;

        public MasterBlockDataStore(MasterBlockDataSettingsScriptableObject masterBlockDataSettingsScriptableObject)
        {
            var settings = masterBlockDataSettingsScriptableObject.BlockSettings;
            BlocksCount = settings.Count;

            for (int i = 0; i < settings.Count; i++)
            {
                var masterBlockData = new MasterBlockData(settings[i], BlocksCount);
                masterBlockDataDictionary.Add(masterBlockData.ID, masterBlockData);
            }

            MasterBlockDataList = masterBlockDataDictionary.Values.ToList();
        }

        public MasterBlockData GetData(BlockID blockID)
        {
            if (masterBlockDataDictionary.TryGetValue(blockID, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
