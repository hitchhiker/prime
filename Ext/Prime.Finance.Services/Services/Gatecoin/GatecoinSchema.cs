using Prime.Finance.Services.Services.Common;

namespace Prime.Finance.Services.Services.Gatecoin
{
    internal class GatecoinSchema
    {
        #region Base

        internal class BaseResponse
        {
            public BaseEntryResponse responseStatus;
        }

        internal class BaseEntryResponse
        {
            public string message;
        }
        #endregion

        #region Public
        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
            public StatusResponse responseStatus;
        }

        internal class TickersResponse
        {
            public TickerEntryResponse[] tickers;
            public StatusResponse responseStatus;
        }

        internal class TickerEntryResponse
        {
            public string currencyPair;
            public decimal open;
            public decimal last;
            public decimal lastQ;
            public decimal high;
            public decimal low;
            public decimal volume;
            public decimal bid;
            public decimal bidQ;
            public decimal ask;
            public decimal askQ;
            public decimal vwap;
            public long createDateTime;
        }

        internal class StatusResponse
        {
            public string message;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public StatusResponse responseStatus;
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }
        #endregion

        #region Private

        internal class BalancesResponse
        {
            public BalanceEntryResponse[] balances;
        }

        internal class BalanceEntryResponse
        {
            public string currency;
            public decimal balance;
            public decimal availableBalance;
            public decimal pendingIncoming;
            public decimal pendingOutgoing;
            public decimal openOrder;
            public bool isDigital;
        }

        internal class OrderResponse
        {
            public string code;
            public string clOrderId;
            public int side;
            public decimal price;
            public decimal initialQuantity;
            public decimal remainingQuantity;
            public string status;
            public string statusDesc;
            public int tranSeqNo;
            public int type;
            public long date;
        }

        internal class NewOrderResponse : BaseResponse
        {
            public string clOrderId;
        }

        internal class WithdrawalResponse : BaseResponse
        {

        }
        #endregion
    }
}
