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

        private static MasterBlockDataStore entity;

        public static void InitialLoad()
        {
            entity = Resources.Load<MasterBlockDataStore>(nameof(MasterBlockDataStore));
        }

        public static MasterBlockData GetData(int id)
        {
            return entity.masterBlockDataList.First(d => d.ID == id);
        }
    }
}
