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

        public IReadOnlyDictionary<BlockShape, Mesh> BlockShapeMeshes => _blockShapeMeshes ??= meshes.ToDictionary(v => v.blockShape, v => v.mesh);
        private Dictionary<BlockShape, Mesh> _blockShapeMeshes;
    }

    [Serializable]
    internal record BlockShapeMesh
    {
        public BlockShape blockShape;
        public Mesh mesh;
    }
}