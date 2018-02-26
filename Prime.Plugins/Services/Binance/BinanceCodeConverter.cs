using System.Collections.Generic;
using Prime.Common;

namespace Prime.Plugins.Services.Binance
{
    public class BinanceCodeConverter : AssetCodeConverterBase
    {
        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return new Dictionary<string, string>()
            {
                {"BCC", "BCH"}
            };
        }
    }
}
