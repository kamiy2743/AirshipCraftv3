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

        private static MasterBlockDataStore _entity;
        private static MasterBlockDataStore Entity => _entity ??=
            Resources.Load<MasterBlockDataStore>(nameof(MasterBlockDataStore));

        public static MasterBlockData GetData(int id)
        {
            return Entity.masterBlockDataList.First(d => d.ID == id);
        }
    }
}
