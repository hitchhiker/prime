using System.Collections.Generic;
using Prime.Common;

namespace Prime.Finance.Services.Services.Globitex
{
    internal class GlobitexCodeConverter : AssetCodeConverterBase
    {
        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "XBT", "BTC" }
            };
        }
    }
}
