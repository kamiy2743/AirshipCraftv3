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
            if (!masterBlockDataDictionary.ContainsKey(id)) return null;
            return masterBlockDataDictionary[id];
        }

        private const string BlockIDScriptPath = "Assets/_Project/Scripts/BlockSystem/BlockID.cs";
        public void GenerateBlockIDScript()
        {
            var code = "namespace BlockSystem { public enum BlockID { ";

            foreach (var masterBlockData in masterBlockDataList)
            {
                code += $"{masterBlockData.Name} = {masterBlockData.ID},";
            }

            code += "}}";
            File.WriteAllText(BlockIDScriptPath, code);
            AssetDatabase.Refresh();
        }
    }

    [CustomEditor(typeof(MasterBlockDataStore))]
    public class MasterBlockDataStoreEditorExt : Editor
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
