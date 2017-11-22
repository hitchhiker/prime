using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Bittrex
{
    internal class BittrexSchema
    {
        internal class BaseResponse<TResult>
        {
            public bool success;
            public string message;
            public TResult result;
        }

        internal class BalancesResponse : BaseResponse<IList<BalanceResponse>> { }

        internal class DepositAddressResponse : BaseResponse<DepositAddressContainerResponse> { }

        internal class MarketEntriesResponse : BaseResponse<IList<MarketEntryResponse>> { }

        internal class TickerResponse : BaseResponse<TickerContainerResponse> { }

        internal class OrderBookResponse : BaseResponse<OrderBookDataResponse> { }

        internal class MarketSummariesResponse : BaseResponse<IList<MarketSummaryEntryResponse>> { }

        internal class MarketSummaryEntryResponse
        {
            public string MarketName;
            public decimal High;
            public decimal Low;
            public decimal Volume;
            public decimal Last;
            public decimal BaseVolume;
            public DateTime TimeStamp;
            public decimal Bid;
            public decimal Ask;
            public int OpenBuyOrders;
            public int OpenSellOrders;
            public decimal PrevDay;
            public DateTime Created;
            public object DisplayMarketName;
        }

        internal class UuidResponse : BaseResponse<UuidResponse>
        {
            public string uuid;
        }

        internal class OpenOrdersResponse : BaseResponse<IList<OpenOrdersEntry>> { }

        internal class OpenOrdersEntry
        {
            public string Uuid;
            public string OrderUuid;
            public string Exchange;
            public string OrderType;
            public decimal Quantity;
            public decimal QuantityRemaining;
            public decimal Limit;
            public decimal CommissionPaid;
            public decimal Price;
            public decimal? PricePerUnit;
            public DateTime? Opened;
            public DateTime? Closed;
            public bool CancelInitiated;
            public bool ImmediateOrCancel;
            public bool IsConditional;
            public string Condition;
            public string ConditionTarget;
        }

        internal class OrderBookDataResponse
        {
            public OrderBookEntryResponse[] buy;
            public OrderBookEntryResponse[] sell;
        }

        internal class OrderBookEntryResponse
        {
            public decimal Quantity;
            public decimal Rate;
        }

        internal class TickerContainerResponse
        {
            public decimal Bid;
            public decimal Ask;
            public decimal Last;
        }

        internal class MarketEntryResponse
        {
            public string MarketCurrency;
            public string BaseCurrency;
            public string Litecoin;
            public string Bitcoin;
            public decimal MinTradeSize;
            public string MarketName;
            public bool IsActive;
            public DateTime Created;
        }

        internal class BalanceResponse
        {
            public string Currency;
            public decimal Balance;
            public decimal Available;
            public decimal Pending;
            public string CryptoAddress;
            public bool Requested;
            public string Uuid;
        }

        internal class DepositAddressContainerResponse
        {
            public string Currency;
            public string Address;
        }
    }
}
