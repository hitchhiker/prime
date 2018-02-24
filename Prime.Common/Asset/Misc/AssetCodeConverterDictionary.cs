using System;
using System.Collections.Generic;

namespace Prime.Common
{
    public class AssetCodeConverterDictionary : AssetCodeConverterBase
    {
        private readonly Dictionary<string, string> _remoteToLocalMap;

        private AssetCodeConverterDictionary() { }

        public AssetCodeConverterDictionary(Dictionary<string, string> remoteToLocalMap)
        {
            _remoteToLocalMap = remoteToLocalMap;
        }

        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return _remoteToLocalMap;
        }

    }
}