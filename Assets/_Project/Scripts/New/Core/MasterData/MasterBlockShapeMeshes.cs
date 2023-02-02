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

        public IReadOnlyDictionary<BlockShapeID, Mesh> BlockShapeMeshDic => _blockShapeMeshDic ??= meshes.ToDictionary(record => record.blockShapeID, record => record.mesh);
        private Dictionary<BlockShapeID, Mesh> _blockShapeMeshDic;
    }

    [Serializable]
    internal record BlockShapeMesh
    {
        public BlockShapeID blockShapeID;
        public Mesh mesh;
    }
}