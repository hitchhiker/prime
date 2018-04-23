using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Quoine
{
    internal class QuoineSchema
    {
        #region Private

        internal class ErrorResponse
        {
            public Dictionary<string,string[]> errors;
            public string message;
        }

        internal class AccountBalanceResponse
        {
            public string currency;
            public decimal balance;
        }
        
        internal class OrderInfoResponse
        {
            public string id;
            public string order_type;
            public int quantity;
            public int disc_quantity;
            public int iceberg_total_quantity;
            public string side;
            public int filled_quantity;
            public decimal price;
            public long created_at;
            public long updated_at;
            public string status;
            public int leverage_level;
            public string source_exchange;
            public int product_id;
            public string product_code;
            public string funding_currency;
            public string currency_pair_code;
            public decimal order_fee;
        }
        #endregion

        #region Public
        internal class ProductResponse
        {
            public int id;
            public decimal market_ask;
            public decimal market_bid;
            public decimal indicator;
            public decimal exchange_rate;
            public decimal? fiat_minimum_withdraw;
            public decimal? taker_fee;
            public decimal? maker_fee;
            public float? low_market_bid;
            public float? high_market_ask;
            public decimal volume_24h;
            public float? last_price_24h;
            public float? last_traded_price;
            public decimal? last_traded_quantity;
            public string product_type;
            public string code;
            public string name;
            public string currency;
            public string currency_pair_code;
            public string symbol;
            public string pusher_channel;
            public string quoted_currency;
            public string base_currency;
        }

        internal class OrderBookResponse
        {
            public decimal[][] buy_price_levels;
            public decimal[][] sell_price_levels;
        }
        #endregion
    }
}
