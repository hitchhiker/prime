using System;
using System.Collections.Generic;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services.BitMarket
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
