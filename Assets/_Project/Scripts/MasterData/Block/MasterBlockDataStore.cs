using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MasterData.Block
{
    public class MasterBlockDataStore : MonoBehaviour
    {
        [SerializeField] private MasterBlockDataStoreScriptableObject resource;

        public static MasterBlockDataStore Instance => _instance;
        private static MasterBlockDataStore _instance;

        void Awake()
        {
            _instance = this;
        }

        public MasterBlockData GetData(int id)
        {
            return resource.MasterBlockDataList.First(d => d.ID == id);
        }
    }
}
