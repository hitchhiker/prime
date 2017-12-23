using Prime.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Globitex
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
