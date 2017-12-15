using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Cryptopia
{
    internal class CryptopiaSchema
    {
        #region Base

        internal class BaseResponse
        {
            public bool Success;
            public string Message;
            public string Error;
        }

        internal class BaseResponse<T> : BaseResponse
        {
            public T Data;
        }

        internal class TradeOrderBaseRequest
        {
            public string Market;
            public long TradePairId;
            public int Count;

            public bool ShouldSerializeTradePairId()
            {
                return String.IsNullOrWhiteSpace(Market);
            }
        }

        #endregion

        #region Private

        internal class BalanceResponse : BaseResponse<BalanceDataResponse[]> { }

        internal class SubmitTradeResponse : BaseResponse<SubmitTradeDataResponse> { }

        internal class BalanceDataResponse
        {
            public int CurrencyId;
            public string Symbol;
            public decimal Total;
            public decimal Available;
            public decimal Unconfirmed;
            public decimal HeldForTrades;
            public decimal PendingWithdraw;
            public string Address;
            public string BaseAddress;
            public string Status;
            public string StatusMessage;
        }

        internal class SubmitTradeDataResponse
        {
            public long OrderId;
            public long[] FilledOrders;
        }

        internal class WithdrawTradeResponse : BaseResponse<int> { }

        internal class OpenOrdersResponse : BaseResponse<OpenOrderResponse[]> { }

        internal class OpenOrderResponse
        {
            public long OrderId;
            public long TradePairId;
            public string Market;
            public string Type;
            public decimal Rate;
            public decimal Amount;
            public decimal Total;
            public decimal Remaining;
            public DateTime TimeStamp;
        }

        internal class TradeHistoryResponse : BaseResponse<TradeHistoryEntryResponse[]> { }

        internal class TradeHistoryEntryResponse
        {
            public long TradeId;
            public long TradePairId;
            public string Market;
            public string Type;
            public decimal Rate;
            public decimal Amount;
            public decimal Total;
            public decimal Fee;
            public DateTime TimeStamp;
        }

        internal class BalanceRequest { }

        internal class SubmitTradeRequest
        {
            public string Market;
            public string Type;
            public decimal Rate;
            public decimal Amount;
        }

        internal class SubmitWithdrawRequest
        {
            public string Currency;

            /// <summary>
            /// (Important!) Address must exist in you AddressBook, can be found in you Security settings page.
            /// </summary>
            public string Address;

            public decimal Amount;
        }

        internal class GetOpenOrdersRequest : TradeOrderBaseRequest { }

        internal class GetTradeHistoryRequest : TradeOrderBaseRequest { }

        #endregion

        #region Public

        internal class AllTickersResponse : BaseResponse<TickerEntryResponse[]>
        {
        }

        internal class TickerResponse : BaseResponse<TickerEntryResponse>
        {
        }

        internal class TickerEntryResponse
        {
            public int TradePairId;
            public string Label;
            public decimal AskPrice;
            public decimal BidPrice;
            public decimal Low;
            public decimal High;
            public decimal Volume;
            public decimal LastPrice;
            public decimal BuyVolume;
            public decimal SellVolume;
            public decimal Change;
            public decimal Open;
            public decimal Close;
            public decimal BaseVolume;
            public decimal BaseBuyVolume;
            public decimal BaseSellVolume;
        }

        #endregion
    }
}
