using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MasterData.Block
{
    [CreateAssetMenu(fileName = "MasterBlockDataStore", menuName = "ScriptableObjects/MasterBlockDataStore")]
    public class MasterBlockDataStore : ScriptableObject
    {
        [SerializeField] private List<MasterBlockData> masterBlockDataList;

        private static Dictionary<int, MasterBlockData> masterBlockDataDictionary = new Dictionary<int, MasterBlockData>();

        public static void InitialLoad()
        {
            var entity = Resources.Load<MasterBlockDataStore>(nameof(MasterBlockDataStore));

            foreach (var masterBlockData in entity.masterBlockDataList)
            {
                masterBlockDataDictionary.Add(masterBlockData.ID, masterBlockData);
            }
        }

        public static MasterBlockData GetData(int id)
        {
            return masterBlockDataDictionary[id];
        }
    }
}
