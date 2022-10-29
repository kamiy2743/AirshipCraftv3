using System;
using UniRx;

namespace Util
{
    /// <summary>
    /// 参照数を管理する
    /// </summary>
    public class ReferenceCounter
    {
        private int count = 0;
        public bool IsFree => count == 0;

        private Subject<Unit> _onAllReferenceReleased;
        public IObservable<Unit> OnAllReferenceReleased => _onAllReferenceReleased ??= new Subject<Unit>();

        public void AddRef()
        {
            lock (this)
            {
                count++;
            }
        }

        public void Release()
        {
            lock (this)
            {
                if (count == 0) throw new System.Exception("参照が0のオブジェクトを解放することはできません");
                count--;
                if (count == 0) _onAllReferenceReleased?.OnNext(default);
            }
        }
    }
}
