using System;

namespace Prime.Finance.Services.Services.CoinCorner
{
    internal class CoinCornerSchema
    {
        #region Private
        internal class BalancesResponse
        {
            public decimal GBPBalance;
            public decimal BTCBalance;
            public decimal EURBalance;
            public decimal GBPReserved;
            public decimal BTCReserved;
            public decimal EURReserved;
            public decimal LTCBalance;
            public decimal DOGEBalance;
            public decimal LTCReserved;
            public decimal DOGEReserved;
        }

        internal class OrderResponse
        {
            public string CreateDate;
            public string OrderId;
            public decimal Amount;
            public decimal Price;
            public string CoinTypeOne;
            public string CoinTypeTwo;
            public string TradeType;
        }

        internal class ErrorResponse
        {
            public string message;
        }
        #endregion

        #region Public
        internal class TickerResponse
        {
            public decimal LastPrice;
            public decimal AvgPrice;
            public decimal LastHigh;
            public decimal LastLow;
            public decimal Volume;
            public decimal BidHigh;
            public decimal BidLow;
            public string Coin;
            public DateTime CreateDate;
        }

        internal class OrderBookResponse
        {
            public string Coin;
            public OrderBookItemResponse[] buys;
            public OrderBookItemResponse[] sells;
        }

        internal class OrderBookItemResponse
        {
            public decimal Amount;
            public decimal Price;
        }
        #endregion
    }
}
