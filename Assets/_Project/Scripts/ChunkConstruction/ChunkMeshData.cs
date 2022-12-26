using System;
using UnityEngine;
using Unity.Collections;
using Util;
using UniRx;

namespace DataObject.Chunk
{
    public class ChunkMeshData : IDisposable
    {
        internal readonly NativeList<Vector3> Vertices = new NativeList<Vector3>(Allocator.Persistent);
        internal readonly NativeList<int> Triangles = new NativeList<int>(Allocator.Persistent);
        internal readonly NativeList<Vector2> UVs = new NativeList<Vector2>(Allocator.Persistent);

        internal IObservable<Unit> OnReleased => _onReleasedSubject;
        private Subject<Unit> _onReleasedSubject = new Subject<Unit>();

        public void Release()
        {
            _onReleasedSubject.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            Vertices.Dispose();
            Triangles.Dispose();
            UVs.Dispose();
        }

        public static implicit operator NativeMeshData(ChunkMeshData meshData)
        {
            if (meshData is null) return null;

            return new NativeMeshData(
                meshData.Vertices,
                meshData.Triangles,
                meshData.UVs);
        }
    }
}
