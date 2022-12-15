using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataObject.Block;
using Zenject;

namespace MasterData.Block
{
    [CreateAssetMenu(fileName = "MasterBlockDataSettingsScriptableObject", menuName = "ScriptableObjects/MasterBlockDataSettingsScriptableObject")]
    public class MasterBlockDataSettingsScriptableObject : ScriptableObjectInstaller<MasterBlockDataSettingsScriptableObject>
    {
        [SerializeField] private List<MasterBlockDataSetting> blockSettings;
        public IReadOnlyList<MasterBlockDataSetting> BlockSettings => blockSettings;

#if UNITY_EDITOR
        private const string BlockIDScriptPath = "Assets/_Project/Scripts/DataObject/Block/BlockID.cs";
        internal void GenerateBlockIDScript()
        {
            var code = "namespace DataObject.Block { public enum BlockID { ";

            foreach (var setting in blockSettings)
            {
                code += $"{setting.Name},";
            }

            code += "}}";
            File.WriteAllText(BlockIDScriptPath, code);
            AssetDatabase.Refresh();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MasterBlockDataSettingsScriptableObject))]
    internal class MasterBlockDataStoreEditorExt : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Apply"))
            {
                var s = target as MasterBlockDataSettingsScriptableObject;
                s.GenerateBlockIDScript();
            }
        }
    }
#endif

    [Serializable]
    public struct MasterBlockDataSetting
    {
        [SerializeField] private string name;
        [SerializeField] private Texture2D texture;

        public BlockID ID => (BlockID)System.Enum.Parse(typeof(BlockID), name);
        public string Name => name;
        public Texture2D Texture => texture;
    }
}
