using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prime.Plugins.Services.Kraken
{
    internal class KrakenSchema
    {
        internal class BaseResponse
        {
            public string[] error;
        }

        internal class BalancesResponse : BaseResponse
        {
            public Dictionary<string, decimal> result;
        }

        internal class AssetPairsResponse : BaseResponse
        {
            public Dictionary<string, AssetPairResponse> result;
        }

        internal class AssetPairResponse
        {
            public string altname;
            public string aclass_base;

            [JsonProperty("base")]
            public string base_c;

            public string aclass_quote;
            public string quote;
            public string lot;
            public int pair_decimals;
            public int lot_decimals;
            public int lot_multiplier;

            public object[] leverage_buy;
            public object[] leverage_sell;

            public decimal[][] fees;
            public decimal[][] fees_maker;

            public string fee_volume_currency;

            public int margin_call;
            public int margin_stop;
        }
    }
}
