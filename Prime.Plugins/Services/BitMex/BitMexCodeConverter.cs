
using System.Collections.Generic;
using Prime.Common;

namespace Prime.Plugins.Services.BitMex
{
    public class BitMexCodeConverter: AssetCodeConverterBase
    {
        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            // Remote codes are case sensitive.
            return new Dictionary<string, string>
            {
                {"XBt", "BTC"}
            };
        }
    }
}