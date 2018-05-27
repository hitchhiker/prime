namespace Prime.Finance.Services.Services.OkCoin
{
    internal class OkCoinSchema
    {
        #region Public
        internal class TickerResponse
        {
            public long date;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal buy;
            public decimal high;
            public decimal last;
            public decimal low;
            public decimal sell;
            public decimal vol;
        }

        internal class OrderBookResponse
        {
            public decimal[][] bids;
            public decimal[][] asks;
        }
        #endregion

        #region Private

        internal class ErrorResponse
        {
            public string error_code;
            public bool result;

        }
        internal class UserInfoResponse
        {
            public UserInfoEntryResponse info;
            public bool result;
        }

        internal class UserInfoEntryResponse
        {
            public AssetResponse asset;
            public CurrenciesResponse borrow;
            public CurrenciesResponse free;
            public CurrenciesResponse freezed;
            public CurrenciesResponse union_fund;
        }

        internal class AssetResponse
        {
            public decimal net;
            public decimal total;
        }

        internal class CurrenciesResponse
        {
            public decimal btc;
            public decimal cny;
            public decimal ltc;
            public decimal eth;
        }

        internal class NewOrderResponse
        {
            public bool result;
            public string order_id;
        }

        internal class OrderResponse
        {
            public bool result;
            public OrderInfoResponse[] orders;
        }

        internal class OrderInfoResponse
        {
            public decimal amount;
            public decimal avg_price;
            public long create_date;
            public decimal deal_amount;
            public string order_id;
            public decimal price;
            public int status;
            public string symbol;
            public string type;
        }
        #endregion
    }
}
