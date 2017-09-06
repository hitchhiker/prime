using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Utility
{
    /// <summary>
    /// https://stackoverflow.com/a/7439725/1318333
    /// An example usage would be storing 5 items. If a 6th object is added, we dequeue the least recent object to keep the size to 5.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueSet<T> : ICollection<T>
    {
        private readonly List<T> _queue = new List<T>();

        private readonly int _maximumSize;

        public QueueSet(int maximumSize)
        {
            if (maximumSize < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumSize));

            this._maximumSize = maximumSize;
        }

        public T Dequeue()
        {
            if (_queue.Count > 0)
            {
                T value = _queue[0];
                _queue.RemoveAt(0);
                return value;
            }
            return default(T);
        }

        public T Peek()
        {
            if (_queue.Count > 0)
            {
                return _queue[0];
            }
            return default(T);
        }

        public void Enqueue(T item)
        {
            if (_queue.Contains(item))
            {
                _queue.Remove(item);
            }
            _queue.Add(item);
            while (_queue.Count > _maximumSize)
            {
                Dequeue();
            }
        }

        public int Count => _queue.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Enqueue(item);
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public bool Contains(T item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T value in _queue)
            {
                if (arrayIndex >= array.Length) break;
                if (arrayIndex >= 0)
                {
                    array[arrayIndex] = value;
                }
                arrayIndex++;
            }
        }

        public bool Remove(T item)
        {
            if (object.Equals(item, Peek()))
            {
                Dequeue();
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}
