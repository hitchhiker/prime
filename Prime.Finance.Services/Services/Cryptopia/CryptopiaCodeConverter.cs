using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance.Services.Services.Cryptopia
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
