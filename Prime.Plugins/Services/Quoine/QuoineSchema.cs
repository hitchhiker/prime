using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Quoine
{
    internal class QuoineSchema
    {
        internal class ProductResponse
        {
            public int id;
            public decimal market_ask;
            public decimal market_bid;
            public decimal indicator;
            public decimal exchange_rate;
            public decimal fiat_minimum_withdraw;
            public decimal taker_fee;
            public decimal maker_fee;
            public float low_market_bid;
            public float high_market_ask;
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
    }
}
