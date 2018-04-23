using System.Collections.Generic;
using Prime.Common;

namespace Prime.Finance.Services.Services.BitMarket
{
    internal class BitMarketCodeConverter : AssetCodeConverterBase
    {
        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "LiteMineX", "LITEMINEX" }
            };
        }
    }
}
