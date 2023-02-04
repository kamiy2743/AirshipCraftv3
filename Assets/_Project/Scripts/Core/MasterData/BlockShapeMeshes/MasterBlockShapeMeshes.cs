using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace MasterData
{
    [CreateAssetMenu(fileName = "MasterBlockShapeMeshes", menuName = "ScriptableObjects/MasterBlockShapeMeshes")]
    internal class MasterBlockShapeMeshes : ScriptableObject
    {
        [SerializeField] private List<BlockShapeMeshRecord> meshes;

        internal IReadOnlyDictionary<BlockShape, Mesh> BlockShapeMeshes => _blockShapeMeshes ??= meshes.ToDictionary(v => v.blockShape, v => v.mesh);
        private Dictionary<BlockShape, Mesh> _blockShapeMeshes;

        [Serializable]
        private record BlockShapeMeshRecord
        {
            public BlockShape blockShape;
            public Mesh mesh;
        }
    }

}