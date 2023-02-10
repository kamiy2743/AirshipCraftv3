using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace MasterData
{
    [CreateAssetMenu(fileName = "MasterBlockShapeMeshes", menuName = "ScriptableObjects/MasterBlockShapeMeshes")]
    class MasterBlockShapeMeshes : ScriptableObject
    {
        [SerializeField] List<BlockShapeMeshRecord> meshes;

        internal IReadOnlyDictionary<BlockShape, Mesh> BlockShapeMeshes => _blockShapeMeshes ??= meshes.ToDictionary(v => v.blockShape, v => v.mesh);
        Dictionary<BlockShape, Mesh> _blockShapeMeshes;

        [Serializable]
        record BlockShapeMeshRecord
        {
            public BlockShape blockShape;
            public Mesh mesh;
        }
    }
}