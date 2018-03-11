using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Acx
{
    internal class AcxSchema
    {
        #region Private
        internal class ErrorResponse
        {
            public ErrorEntryResponse error;
        }

        internal class ErrorEntryResponse
        {
            public int code;
            public string message;
        }

        internal class UserInfoResponse
        {
            public string sn;
            public string name;
            public string email;
            public bool activated;
            public AccountResponse[] accounts;
        }

        internal class OrderInfoResponse
        {
            public string id;
            public string side;
            public decimal price;
            public decimal avg_price;
            public string state;
            public string market;
            public long created_at;
            public decimal volume;
            public decimal remaining_volume;
            public decimal executed_volume;
            public OrderInfoEntryResponse[] trades;
        }

        internal class OrderInfoEntryResponse
        {
            public string id;
            public decimal price;
            public decimal volume;
            public string market;
            public long created_at;
            public string side;
        }

        internal class NewOrderResponse
        {
            public string id;
        }

        internal class AccountResponse
        {
            public string currency;
            public decimal balance;
            public double locked;
        }
        #endregion

        #region Public
        internal class AllTickersResponse : Dictionary<string, TickersResponse>
        {
        }

        internal class TickerResponse
        {
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickersResponse
        {
            public string name;
            public string base_unit;
            public string quote_unit;
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public string fund_id;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal open;
            public decimal close;
            public decimal volume;
            public decimal volume_traded;
            public decimal bid;
            public decimal ask;
            public DateTime date;
        }

        internal class MarketResponse
        {
            public string id;
            public string name;
            public string base_unit;
            public string quote_unit;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public decimal[][] asks;
            public decimal[][] bids;
        }
        #endregion
    }
}
