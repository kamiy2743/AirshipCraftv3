using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataObject.Block;

namespace MasterData.Block
{
    [CreateAssetMenu(fileName = "MasterBlockDataStore", menuName = "ScriptableObjects/MasterBlockDataStore")]
    public class MasterBlockDataStore : ScriptableObject
    {
        [SerializeField] private Material blockMaterial;
        [SerializeField] private List<MasterBlockData> masterBlockDataList;

        private static Dictionary<BlockID, MasterBlockData> masterBlockDataDictionary = new Dictionary<BlockID, MasterBlockData>();
        public static List<MasterBlockData> MasterBlockDataList => masterBlockDataDictionary.Values.ToList();

        public static void InitialLoad()
        {
            masterBlockDataDictionary.Clear();

            var entity = Resources.Load<MasterBlockDataStore>(nameof(MasterBlockDataStore));
            var blockCount = entity.masterBlockDataList.Count;

            for (int i = 0; i < entity.masterBlockDataList.Count; i++)
            {
                var masterBlockData = entity.masterBlockDataList[i];
                masterBlockData.Init(i, blockCount);
                masterBlockDataDictionary.Add(masterBlockData.ID, masterBlockData);
            }

            // マテリアルにテクスチャをセット
            new BlockMaterialInitializer(entity.blockMaterial, blockCount);
        }

        public static MasterBlockData GetData(BlockID blockID)
        {
            if (!masterBlockDataDictionary.ContainsKey(blockID)) return null;
            return masterBlockDataDictionary[blockID];
        }

#if UNITY_EDITOR
        private const string BlockIDScriptPath = "Assets/_Project/Scripts/DataObject/Block/BlockID.cs";
        internal void GenerateBlockIDScript()
        {
            var code = "namespace DataObject.Block { public enum BlockID { ";

            foreach (var masterBlockData in masterBlockDataList)
            {
                code += $"{masterBlockData.Name},";
            }

            code += "}}";
            File.WriteAllText(BlockIDScriptPath, code);
            AssetDatabase.Refresh();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MasterBlockDataStore))]
    internal class MasterBlockDataStoreEditorExt : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Apply"))
            {
                var s = target as MasterBlockDataStore;
                s.GenerateBlockIDScript();
            }
        }
    }
#endif
}
