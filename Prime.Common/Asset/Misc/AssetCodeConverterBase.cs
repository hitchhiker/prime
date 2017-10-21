using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public abstract class AssetCodeConverterBase : IAssetCodeConverter
    {
        public virtual string ToLocalCode(string remoteCode)
        {
            var u = remoteCode.ToUpper().Trim();
            return RemoteToLocal.Get(u, u);
        }

        public virtual string ToRemoteCode(Asset localAsset)
        {
            return LocalToRemote.Get(localAsset.ShortCode, localAsset.ShortCode);
        }

        private Dictionary<string,string> _remoteToLocal;
        public Dictionary<string,string> RemoteToLocal => _remoteToLocal ?? (_remoteToLocal = GetRemoteLocalDictionary());

        private Dictionary<string, string> _localToRemote;
        public Dictionary<string, string> LocalToRemote => _localToRemote ?? (_localToRemote = GetLocalRemoteDictionary());

        protected abstract Dictionary<string, string> GetRemoteLocalDictionary();

        private Dictionary<string, string> GetLocalRemoteDictionary()
        {
            var kv = RemoteToLocal.Values.GroupBy(x => x).FirstOrDefault(x => x.Count() > 1);

            if (kv?.Key!=null)
                throw new Exception("Value: " + kv.Key + " occurs more than once in this dictionary, therefore the dictionary cannot be inverted. " + GetType());

            return RemoteToLocal.ToDictionary(x => x.Value, y => y.Key);
            
/*
            var d = new Dictionary<string, string>();
            foreach (var k in RemoteToLocal)
            {
                var key = k.Value;
                if (d.ContainsKey(key))
                    continue;
                d.Add(key, k.Key);
            }
            return d;*/
        }
    }
}