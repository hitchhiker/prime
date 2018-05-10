using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Exmo
{
    internal class ExmoSchema
    {
        #region Base

        internal class BaseResponse
        {
            public bool result;
            public string error;
        }

        #endregion

        #region Private

        internal class UserInfoResponse
        {
            public long uid;
            public long server_date;
            public Dictionary<string, decimal> balances;
            public Dictionary<string, decimal> reserved;
        }

        internal class NewOrderResponse : BaseResponse
        {
            public string order_id;
        }

        internal class ActiveOrdersResponse : Dictionary<string, OrderInfoResponse[]>
        {
        }

        internal class OrderInfoResponse
        {
            public string order_id;
            public long created;
            public string type;
            public string pair;
            public decimal price;
            public decimal quantity;
            public decimal amount;
        }

        internal class WithdrawalResponse : BaseResponse
        {
            public string task_id;
        }

        #endregion

        #region Public
        internal class TickerResponse : Dictionary<string, TickerEntryResponse> { }

        internal class OrderBookResponse : Dictionary<string, OrderBookEntryResponse> { }

        internal class TickerEntryResponse
        {
            public decimal avg;
            public decimal high;
            public decimal low;
            public decimal last_trade;
            public decimal vol;
            public decimal vol_curr;
            public decimal buy_price;
            public decimal sell_price;
            public long updated;
        }

        internal class OrderBookEntryResponse
        {
            public decimal ask_quantity;
            public decimal ask_amount;
            public decimal ask_top;
            public decimal bid_quantity;
            public decimal bid_amount;
            public decimal bid_top;
            public decimal[][] ask;
            public decimal[][] bid;
        }
        #endregion
    }
}
