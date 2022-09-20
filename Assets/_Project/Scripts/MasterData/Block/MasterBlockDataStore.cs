using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MasterData.Block
{
    [CreateAssetMenu(fileName = "MasterBlockDataStore", menuName = "ScriptableObjects/MasterBlockDataStore")]
    public class MasterBlockDataStore : ScriptableObject
    {
        [SerializeField] private Material blockMaterial;
        [SerializeField] private List<MasterBlockData> masterBlockDataList;

        private static Dictionary<int, MasterBlockData> masterBlockDataDictionary = new Dictionary<int, MasterBlockData>();

        public static void InitialLoad()
        {
            masterBlockDataDictionary.Clear();

            var entity = Resources.Load<MasterBlockDataStore>(nameof(MasterBlockDataStore));
            var blockCount = entity.masterBlockDataList.Count;

            for (int i = 0; i < entity.masterBlockDataList.Count; i++)
            {
                var masterBlockData = entity.masterBlockDataList[i];
                masterBlockData.Init(i, blockCount);
                masterBlockDataDictionary.Add((int)masterBlockData.ID, masterBlockData);
            }

            // マテリアルにテクスチャをセット
            new BlockMaterialInitializer(entity.blockMaterial, blockCount);
        }

        public static MasterBlockData GetData(BlockID blockID)
        {
            return GetData((int)blockID);
        }

        public static MasterBlockData GetData(int blockID)
        {
            if (!masterBlockDataDictionary.ContainsKey(blockID)) return null;
            return masterBlockDataDictionary[blockID];
        }

        private const string BlockIDScriptPath = "Assets/_Project/Scripts/MasterData/Block/BlockID.cs";
        internal void GenerateBlockIDScript()
        {
            var code = "namespace MasterData.Block { public enum BlockID { ";

            foreach (var masterBlockData in masterBlockDataList)
            {
                code += $"{masterBlockData.Name},";
            }

            code += "}}";
            File.WriteAllText(BlockIDScriptPath, code);
            AssetDatabase.Refresh();
        }
    }

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
}
