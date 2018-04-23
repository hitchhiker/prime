using System.Collections.Generic;
using Prime.Common;

namespace Prime.Finance.Services.Services.BitMex
{
    public class BitMexCodeConverter: AssetCodeConverterBase
    {
        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return new Dictionary<string, string>
            {
                {"XBT", "BTC"}
            };
        }
    }
}