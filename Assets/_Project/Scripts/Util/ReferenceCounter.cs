namespace Util
{
    /// <summary>
    /// 参照数を管理する
    /// </summary>
    public class ReferenceCounter
    {
        private int count = 0;
        public bool IsFree => count == 0;

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
            }
        }
    }
}
