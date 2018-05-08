using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Bitsane
{
    internal class BitsaneSchema
    {
        #region Public
        internal class TickerResponse : Dictionary<string, TickerEntryResponse>
        {
        }

        internal class TickerEntryResponse
        {
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public decimal high24hr;
            public decimal low24hr;
            public decimal euroEquivalent;
            public decimal bitcoinEquivalent;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
            public long timestamp;
        }

        internal class OrderBookResponse
        {
            public int statusCode;
            public string statusText;
            public OrderBookEntryResponse result;
        }
        #endregion

        #region Private
        internal class BaseResponse
        {
            public int statusCode;
            public string statusText;
        }

        internal class ErrorResponse
        {
            public int statusCode;
            public string statusText;
            public ErrorEntryResponse result;
        }

        internal class ErrorEntryResponse
        {
            public string message;
        }

        internal class BalanceResponse : BaseResponse
        {
            public Dictionary<string, BalanceEntryResponse> result;
        }

        internal class BalanceEntryResponse
        {
            public string currency;
            public decimal amount;
            public decimal locked;
            public decimal total;
        }

        internal class WithdrawalResponse : BaseResponse
        {
            public WithdrawalEntryResponse result;
        }

        internal class WithdrawalEntryResponse
        {
            public string withdrawal_id;
        }

        internal class NewOrderResponse : BaseResponse
        {
            public OrderEntryResponse result;
        }

        internal class OrderEntryResponse
        {
            public string order_id;
        }

        internal class OrderInfoResponse : BaseResponse
        {
            public OrderInfoEntryResponse[] result;
        }

        internal class OrderInfoEntryResponse
        {
            public string id;
            public string pair;
            public decimal price;
            public string side;
            public string type;
            public long timestamp;
            public bool is_closed;
            public bool is_hidden;
            public decimal executed_amount;
            public decimal remaining_amount;
            public decimal original_amount;
        }

        #endregion
    }
}
