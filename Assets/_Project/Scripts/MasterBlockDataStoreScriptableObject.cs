using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData.Block
{
    [CreateAssetMenu(fileName = "MasterBlockDataStore", menuName = "ScriptableObjects/MasterBlockDataStore")]
    internal class MasterBlockDataStoreScriptableObject : ScriptableObject
    {
        [SerializeField] private List<MasterBlockData> masterBlockDataList;
        internal IReadOnlyList<MasterBlockData> MasterBlockDataList => masterBlockDataList;
    }
}
