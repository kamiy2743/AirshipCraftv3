using System;
using UniRx;

namespace Util
{
    public class ReferenceCounter
    {
        private int count = 0;
        public bool IsFree => count == 0;

        private Subject<Unit> _onAllReferenceReleased;
        public IObservable<Unit> OnAllReferenceReleased => _onAllReferenceReleased ??= new Subject<Unit>();

        public void AddRef()
        {
            var spinLock = new FastSpinLock();
            try
            {
                spinLock.Enter();
                count++;
            }
            finally
            {
                spinLock.Exit();
            }
        }

        public void Release()
        {
            var spinLock = new FastSpinLock();
            try
            {
                spinLock.Enter();
                if (count == 0) throw new System.Exception("参照が0のオブジェクトを解放することはできません");
                count--;
                if (count == 0) _onAllReferenceReleased?.OnNext(default);
            }
            finally
            {
                spinLock.Exit();
            }
        }
    }
}
