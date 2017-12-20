using System;
using System.Collections.Generic;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services.IndependentReserve
{
    internal class IndependentReserveCodeConverter : AssetCodeConverterBase
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
