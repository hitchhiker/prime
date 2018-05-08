using Newtonsoft.Json.Serialization;

namespace Prime.Finance.Services.Services.Luno
{
    internal class LunoSchema
    {
        #region Base
        internal class ErrorResponse
        {
            public string error;
            public string error_code;
        }
        #endregion

        #region Private
        internal class BalancesResponse
        {
            public BalanceEntryResponse[] balance;
        }

        internal class BalanceEntryResponse
        {
            public string account_id;
            public string asset;
            public decimal balance;
            public decimal reserved;
            public decimal unconfirmed;
            public string name;
        }

        internal class OrderResponse
        {
            public string order_id;
            public long creation_timestamp;
            public long expiration_timestamp;
            public long completed_timestamp;
            public string type;
            public string state;
            public decimal limit_price;
            public decimal limit_volume;
            public decimal counter;
            public decimal fee_base;
            public decimal fee_counter;
        }

        internal class WithdrawalResponse
        {
            public bool success;
        }
        #endregion

        #region Public
        internal class AllTickersResponse
        {
            public TickerResponse[] tickers;
        }

        internal class TickerResponse
        {
            public string pair;
            public decimal bid;
            public decimal ask;
            public decimal last_trade;
            public decimal rolling_24_hour_volume;
            public long timestamp;
        }

        internal class OrderBookItemResponse
        {
            public decimal volume;
            public decimal price;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }
        #endregion
    }
}
