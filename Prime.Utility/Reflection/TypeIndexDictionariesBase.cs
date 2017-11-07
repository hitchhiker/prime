using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Prime.Utility
{
    /// <summary>
    /// Contains 2 seperate *oppositely indexed* type dictionaries for extremely fast lookup.
    /// </summary>\
    public abstract class TypeIndexDictionariesBase<T> : IEnumerable<Type>
    {
        protected TypeIndexDictionariesBase(IEnumerable<Type> types, Func<Type, T> hashFunction)
        {
            Types = types.ToList();
            try
            {
                _typeLookup = Types.ToDictionary(hashFunction, t => t);
            }
            catch
            {
                var table = Types.ToDictionary(t => t, hashFunction);
                var dupes = new UniqueList<string>();
                foreach (var t in table)
                {
                    if (table.Values.Count(x=>x.Equals(t.Value))>1)
                        dupes.Add(t.Key.FullName);
                }

                var dup = string.Join(", ", dupes);
                throw new Exception("Type(s) '" + dup + "' cannot be added to " + GetType() + " because of a hash collision.");
            }
            _hashLookup = _typeLookup.ToDictionary(k => k.Value, v => v.Key);
        }


        private readonly Dictionary<Type, T> _hashLookup = new Dictionary<Type, T>();
        private readonly Dictionary<T, Type> _typeLookup = new Dictionary<T, Type>();
        protected readonly List<Type> Types;
        
        public List<T> Get(IEnumerable<Type> type)
        {
            return type.Select(Get).ToList();
        }

        public virtual T Get(Type type)
        {
            if (!_hashLookup.ContainsKey(type))
                throw new Exception(GetType() + " Nothing in catalogue for type: " + type);

            return _hashLookup.Get(type);
        }

        public virtual Type Get(T hash)
        {
            if (Equals(hash, default(T)))
                return null;

            if (!_typeLookup.ContainsKey(hash))
                throw new Exception(GetType() + " Nothing in catalogue for hash: " + hash);

            var type = _typeLookup.Get(hash);
            return type;
        }

        public TItem GetUninitialized<TItem>(T hash) where TItem : class
        {
            if (Equals(hash, default(T)))
                return null;

            return GetUninitialized<TItem>(Get(hash));
        }

        public TItem GetUninitialized<TItem>(Type type) where TItem : class
        {
            return FormatterServices.GetUninitializedObject(type) as TItem;
        }

        public TItem GetInstance<TItem>() where TItem : class
        {
            return GetInstance<TItem>(typeof(TItem));
        }

        public TItem GetInstance<TItem>(T hash) where TItem : class
        {
            if (Equals(hash, default(T)))
                return null;

            return GetInstance<TItem>(Get(hash));
        }

        public TItem GetInstance<TItem>(Type type) where TItem : class
        {
            return Reflection.InstanceAny(type) as TItem;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return Types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}