using System;
using System.Collections.Generic;
using Prime.Core;

namespace plugins
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