using System;
using System.Collections.Generic;

namespace Utils
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        readonly T[] _array;
        readonly IComparer<T> _comparer;
        public int Count { get; private set; } = 0;

        public PriorityQueue(int capacity, IComparer<T> comparer = null)
        {
            _array = new T[capacity];
            _comparer = comparer;
        }

        /// <summary>
        /// 要素を挿入する
        /// </summary>
        public void Enqueue(T item)
        {
            _array[Count] = item;
            Count += 1;

            var n = Count - 1;                  // 末尾(追加した)のノードの番号
            while (n != 0)
            {
                var parent = (n - 1) / 2;       // 親ノードの番号

                if (Compare(_array[n], _array[parent]) > 0)
                {
                    Swap(n, parent);
                    n = parent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 優先度の一番高いものを取り出す
        /// </summary>
        public T Dequeue()
        {
            Swap(0, Count - 1);            // 先頭要素を末尾にする
            Count -= 1;

            var parent = 0;                     // 親ノードの番号
            while (true)
            {
                var child = 2 * parent + 1;     // 子ノードの番号
                if (child > Count - 1) break;

                // 値の大きい方の子を選ぶ
                if (child < Count - 1 && Compare(_array[child], _array[child + 1]) < 0) child += 1;

                // 子の方が親より大きければ入れ替える
                if (Compare(_array[parent], _array[child]) < 0)
                {
                    Swap(parent, child);
                    parent = child;
                }
                else
                {
                    break;
                }
            }

            return _array[Count];
        }

        /// <summary>
        /// 大きいものから列挙していく
        /// withPop = falseのときは順番関係なく取り出しもしないが素早く全要素を取得できる　
        /// </summary>
        public IEnumerable<T> GetAllElements(bool withPop = true)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                if (withPop) yield return Dequeue();
                else yield return _array[count - i - 1];
            }
        }

        int Compare(T a, T b)
        {
            if (_comparer == null) return a.CompareTo(b);
            return _comparer.Compare(a, b);
        }

        void Swap(int a, int b)
        {
            (_array[a], _array[b]) = (_array[b], _array[a]);
        }
    }
}