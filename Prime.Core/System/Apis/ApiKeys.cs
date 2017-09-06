using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Core;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class ApiKeys : IReadOnlyList<ApiKey>, ISerialiseAsObject
    {
        public ApiKeys() { }

        [Bson]
        private UniqueList<ApiKey> Keys { get; set; } = new UniqueList<ApiKey>();

        [Bson]
        public bool FsDone { get; private set; }

        public ApiKey GetFirst(INetworkProvider provider)
        {
            return Keys.FirstOrDefault();
        }

        internal void CollectFilesystem(INetworkProvider provider)
        {
            var fi = new FileInfo($"../../../{provider.Network.NameLowered}.api.txt");
            
            if (!fi.Exists)
                return;

            var apik = ApiKey.FromFile(fi);
            if (apik == null)
                return;

            Keys.Add(apik);
            FsDone = true;
        }

        public void Remove(ApiKey key)
        {
            Keys.Remove(key);
        }

        public void Add(ApiKey key)
        {
            Keys.Add(key, true);
        }

        public IEnumerator<ApiKey> GetEnumerator()
        {
            return Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Keys).GetEnumerator();
        }

        public int Count => Keys.Count;

        public ApiKey this[int index] => Keys[index];
    }
}
