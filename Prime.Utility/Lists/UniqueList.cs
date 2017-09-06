using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Prime.Utility
{
    /// <summary>
    /// ignores items during an add if they're already present
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueList<T> : DirtyList<T>, IUniqueList<T> where T : IEquatable<T>
    {
        private readonly Object _writeLock = new Object();

        public UniqueList() { }

        public UniqueList(IEnumerable<T> items)
        {
            if (items == null)
                return;
            foreach (var i in items)
                InternalAdd(i);
        }
        
        public virtual bool Add(T item, bool readd = false)
        {
            return InternalAdd(item, readd);
        }

        private bool InternalAdd(T item, bool readd = false)
        {
            if (Equals(item, default(T)))
                return false;

            lock (_writeLock)
            {
                if (readd)
                    Remove(item);

                if (this.Any(x => x.Equals(item)))
                    return false;

                base.Add(item);
            }

            return true;
        }

        public override void Add(T item)
        {
            if (Equals(item, default(T)))
                return;

            InternalAdd(item);
        }

        public override void Insert(int index, T item)
        {
            lock(_writeLock) {
                if (this.Any(x => x.Equals(item)))
                    return;
                
                base.Insert(index, item);
            }
        }

        public override bool AddRange(IEnumerable<T> items)
        {
            return AddRange(items, false);
        }

        public bool AddRange(IEnumerable<T> items, bool readd)
        {
            if (items == null)
                return false;
            var changed = false;
            foreach (var item in items)
            {
                changed = Add(item,readd) || changed;
            }
            return changed;
        }

        public IEnumerable<T> Distinct() { return this; }

        public T Single(T item)
        {
            return this.FirstOrDefault(x => x.Equals(item));
        }
    }
}
