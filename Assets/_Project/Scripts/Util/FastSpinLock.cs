using System.Threading;
using System.Runtime.CompilerServices;

namespace Util
{
    public struct FastSpinLock
    {
        private const int SYNC_ENTER = 1;
        private const int SYNC_EXIT = 0;
        private int _syncFlag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enter()
        {
            if (Interlocked.CompareExchange(ref _syncFlag, SYNC_ENTER, SYNC_EXIT) == SYNC_ENTER)
            {
                Spin();
            }
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exit() => Volatile.Write(ref _syncFlag, SYNC_EXIT);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Spin()
        {
            var spinner = new SpinWait();
            spinner.SpinOnce();
            while (Interlocked.CompareExchange(ref _syncFlag, SYNC_ENTER, SYNC_EXIT) == SYNC_ENTER)
            {
                spinner.SpinOnce();
            }
        }
    }
}
