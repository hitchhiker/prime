using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prime.Utility
{
    public class DirtyList<T> : IList<T>, IReadOnlyList<T>, IList
    {
        private readonly List<T> _list;

        public DirtyList()
        {
            _list = new List<T>();
        }

        public virtual IEnumerator<T> GetEnumerator() { return _list.GetEnumerator(); }

        public virtual void Add(T item)
        {
            _list.Add(item);
            OnChange();
        }

        public virtual bool AddRange(IEnumerable<T> collection)
        {
            _list.InsertRange(_list.Count, collection);
            OnChange();
            return true;
        }

        int IList.Add(object value)
        {
            _list.Add((T)value);
            return 1;
        }

        bool IList.Contains(object value)
        {
            return _list.Contains((T)value);
        }

        public virtual void Clear()
        {
            _list.Clear();
            OnChange();
        }

        int IList.IndexOf(object value)
        {
            return _list.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            _list.Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            _list.Remove((T)value);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            _list.Insert(index, item);
            OnChange();
        }

        public virtual bool Remove(T item)
        {
            var r = _list.Remove(item);
            OnChange();
            return r;
        }

        public void RemoveNoDirty(T item)
        {
            _list.Remove(item);
        }

        public virtual int RemoveAll(Predicate<T> match)
        {
            var r = _list.RemoveAll(match);
            if (r > 0)
                OnChange();
            return r;
        }

        public virtual int RemoveAll(IEnumerable<T> list)
        {
            var c = 0;
            foreach (var i in list)
            {
                _list.Remove(i);
                c++;
            }
            if (c>0)
                OnChange();
            return c;
        }
        public void Reverse()
        {
            _list.Reverse();
            OnChange();
        }

        public bool Exists(Predicate<T> match)
        {
            return _list.Exists(match);
        }

        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }
        
        public List<T> FindAll(Predicate<T> match)
        {
            return _list.FindAll(match);
        }

        public void Sort(IComparer<T> comparer)
        {
            _list.Sort(comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _list.Sort(index, count, comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            _list.Sort(comparison);
        }

        public void CopyTo(Array array, int index)
        {
            _list.ToArray().CopyTo(array, index);
        }

        public int Count { get { return _list.Count; } }

        public object SyncRoot { get { return ((IList) _list).SyncRoot; } }

        public bool IsSynchronized { get { return ((IList)_list).IsSynchronized; } }

        public virtual void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            OnChange();
        }

        object IList.this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = (T)value; }
        }

        public virtual T this[int index]
        {
            get { return _list[index]; }
            set
            {
                IsDirty = (IsDirty || !_list[index].Equals(value));
                _list[index] = value;
                if (IsDirty)
                    OnChange();
            }
        }

        public bool IsReadOnly => false;

        public bool IsFixedSize
        {
            get { return ((IList)_list).IsFixedSize; } 
        }

        public bool IsDirty { get; set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void OnChange()
        {
            IsDirty = true;
        }
    }
}
