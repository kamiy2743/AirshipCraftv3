using System;
using UniRx;
using System.Threading;

namespace Util
{
    public class ReferenceCounter : IDisposable
    {
        private int count = 0;

        public IObservable<Unit> OnAllReferenceReleased => _onAllReferenceReleased ??= new Subject<Unit>();
        private Subject<Unit> _onAllReferenceReleased;

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public void AddRef()
        {
            Interlocked.Increment(ref count);
        }

        public void Release()
        {
            var after = Interlocked.Decrement(ref count);
            if (count < 0) throw new System.Exception("参照が0のオブジェクトを解放することはできません");
            if (count == 0) _onAllReferenceReleased?.OnNext(Unit.Default);
        }

        public bool IsFree()
        {
            rwLock.EnterReadLock();
            var result = count == 0;
            rwLock.ExitReadLock();
            return result;
        }

        public void Dispose()
        {
            rwLock.Dispose();
        }
    }
}
