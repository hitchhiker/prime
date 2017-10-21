using System;
using System.Collections.Generic;
using Prime.Common;

namespace Prime.Plugins.Services.Kraken
{
    public class KrakenCodeConverterBase : AssetCodeConverterBase
    {
        private KrakenCodeConverterBase() {}

        public static KrakenCodeConverterBase I => Lazy.Value;
        private static readonly Lazy<KrakenCodeConverterBase> Lazy = new Lazy<KrakenCodeConverterBase>(()=>new KrakenCodeConverterBase());

        protected override Dictionary<string, string> GetRemoteLocalDictionary()
        {
            return new Dictionary<string, string>
            {
                {"XBT", "BTC"}
            };
        }

        public override string ToLocalCode(string remoteCode)
        {
            if (remoteCode.Length == 4 && remoteCode.StartsWith("X", StringComparison.OrdinalIgnoreCase))
                return base.ToLocalCode(remoteCode.Substring(1));

            return base.ToLocalCode(remoteCode);
        }
    }
}