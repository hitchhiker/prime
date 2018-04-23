using System.Collections.Generic;
using Prime.Common;

namespace Prime.Finance.Services.Services.ItBit
{
    public class ItBitCodeConverter : AssetCodeConverterBase
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
