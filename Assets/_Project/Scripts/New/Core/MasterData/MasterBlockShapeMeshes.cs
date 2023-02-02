using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace MasterData
{
    [CreateAssetMenu(fileName = "MasterBlockShapeMeshes", menuName = "ScriptableObjects/MasterBlockShapeMeshes")]
    public class MasterBlockShapeMeshes : ScriptableObject
    {
        [SerializeField] private List<BlockShapeMesh> meshes;

        public IReadOnlyDictionary<BlockShape, Mesh> BlockShapeMeshDic => _blockShapeMeshDic ??= meshes.ToDictionary(record => record.blockShape, record => record.mesh);
        private Dictionary<BlockShape, Mesh> _blockShapeMeshDic;
    }

    [Serializable]
    internal record BlockShapeMesh
    {
        public BlockShape blockShape;
        public Mesh mesh;
    }
}