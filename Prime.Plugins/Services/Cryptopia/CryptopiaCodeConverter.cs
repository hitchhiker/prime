using System;
using System.Collections.Generic;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services.Cryptopia
{
    internal class CryptopiaCodeConverter : AssetCodeConverterBase
    {
        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "$$$", "USD" }
            };
        }
    }
}
