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
        internal class ErrorResponse
        {
            public string[] error;
        }

        internal class BaseResponse<TResultKey, TResultObject> : ErrorResponse
        {
            public Dictionary<TResultKey, TResultObject> result;
        }

        internal class BalancesResponse : BaseResponse<string, decimal> { }

        internal class AssetPairsResponse : BaseResponse<string, AssetPairResponse> { }

        internal class TickersInformationResponse : BaseResponse<string, TickerInformationResponse> { }

        internal class DepositMethodsResponse : BaseResponse<string, DepositMethodResponse> { }

        internal class DepositAddressesResponse : BaseResponse<string, DepositAddressResponse> { }

        internal class DepositAddressResponse
        {
            public string address;
            public long expiretm;

            [JsonProperty("new")]
            public bool new_address;
        }

        internal class OhlcResponse : ErrorResponse
        {
            public OhlcResultResponse result;
        }

        internal class OhlcResultResponse
        {
            public Dictionary<string, OhlcDataRespose[]> pairs;
            public long last;
        }

        internal class OhlcDataRespose
        {
            public decimal time;
            public decimal open;
            public decimal high;
            public decimal low;
            public decimal close;
            public decimal vwap;
            public decimal volume;
            public decimal count;
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

        internal class TickerInformationResponse
        {
            public decimal[] a;
            public decimal[] b;
            public decimal[] c;
            public decimal[] v;
            public decimal[] p;

            public int[] t;

            public decimal[] l;
            public decimal[] h;

            public decimal o;
        }

        internal class DepositMethodResponse
        {
            public string method;
            public string limit;
            public decimal fee;

            [JsonProperty("address-setup-fee")]
            public bool? address_setup_fee;
        }
    }
}
