using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance.Services.Services.Binance
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
