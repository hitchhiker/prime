using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Prime.Plugins.Services.Bitfinex
{
    internal class BitfinexSchema
    {
        #region Base

        internal class BaseResponse
        {
            public string message;
        }

        internal class FeesBaseResposnse : BaseResponse
        {
            public decimal maker_fees;
            public decimal taker_fees;
        }

        internal class BaseRequest
        {
            [JsonProperty(Order = -2)]
            public string request;
            [JsonProperty(Order = -2)]
            public string nonce;
        }

        internal interface IClassDescriptor
        {
             string ClassName { get; }
        }

        #endregion

        #region Private

        internal class AccountInfoResponse : List<AccountInfoDataResponse> { }

        internal class AccountInfoDataResponse : FeesBaseResposnse
        {
            public FeesResponse fees;
        }

        internal class FeesResponse : List<FeeResponse> { }

        internal class FeeResponse : FeesBaseResposnse
        {
            public string pairs;
        }

        internal class WalletBalanceResponse : List<WalletBalancesResponse> { }

        internal class WalletBalancesResponse
        {
            public string type;
            public string currency;
            public decimal amount;
            public decimal available;
        }

        internal class NewOrderResponse : BaseResponse
        {
            public long id;
            public string symbol;
            public string exchange;
            public decimal price;
            public decimal avg_execution_price;
            public string side;
            public string type;
            public string timestamp;
            public bool is_live;
            public bool is_cancelled;
            public bool is_hidden;
            public bool was_forced;
            public decimal original_amount;
            public decimal remaining_amount;
            public decimal executed_amount;
            public long order_id;
        }

        internal class WithdrawalsResponse : List<WithdrawalResponse>
        {
            public string status;
            public string message;
            public long withdrawal_id;
        }

        internal class WithdrawalResponse : BaseResponse
        {
            public string status;
            // public string message; // Inherited from BaseResponse.
            public long withdrawal_id;
        }

        internal class OrderStatusResponse : BaseResponse
        {
            public long id;
            public string symbol;
            public string exchange;
            public decimal? price; // "(can be null for market orders)".
            public decimal avg_execution_price;
            public string side;
            public string type;
            public double timestamp;
            public bool is_live;
            public bool is_cancelled;
            public bool is_hidden;
            public string oco_order;
            public bool was_forced;
            public decimal original_amount;
            public decimal remaining_amount;
            public decimal executed_amount;
        }

        internal class OrderStatusRequest : BaseRequest
        {
            public long order_id;

            internal class Descriptor : OrderStatusRequest, IClassDescriptor
            {
                public string ClassName => nameof(OrderStatusRequest);
            }
        }

        internal class AccountInfoRequest : BaseRequest
        {
            internal class Descriptor : AccountInfoRequest, IClassDescriptor
            {
                public string ClassName => nameof(AccountInfoRequest);
            }
        }

        internal class WalletBalancesRequest : BaseRequest
        {
            internal class Descriptor : AccountInfoRequest, IClassDescriptor
            {
                public string ClassName => nameof(WalletBalancesRequest);
            }
        }

        internal class NewOrderRequest : BaseRequest
        {
            public NewOrderRequest()
            {
                is_hidden = false;
                use_all_available = 0;
                ocoorder = false;
                buy_price_oco = 0;
                sell_price_oco = 0;
                type = "exchange limit";
                exchange = "bitfinex";
            }

            public string symbol; 
            public string amount;
            public string price;
            public string exchange;
            public string side;
            public string type;
            
            public bool is_hidden;
            public bool is_postonly;
            public int use_all_available;
            public bool ocoorder;
            public int buy_price_oco;
            public int sell_price_oco;

            internal class Descriptor : NewOrderRequest, IClassDescriptor
            {
                public string ClassName => nameof(NewOrderRequest);
            }
        }

        internal class WithdrawalRequest : BaseRequest
        {
            public string withdraw_type;
            public string walletselected;
            public string amount;
            public string address;
            public string payment_id;

            public bool ShouldSerializepayment_id()
            {
                return !String.IsNullOrWhiteSpace(payment_id);
            }

            internal class Descriptor : WithdrawalRequest, IClassDescriptor
            {
                public string ClassName => nameof(WithdrawalRequest);
            }
        }

        #endregion

        #region Public

        internal class TickerResponse
        {
            public decimal mid;
            public decimal bid;
            public decimal ask;
            public decimal last_price;
            public decimal low;
            public decimal high;
            public decimal volume;
            public decimal timestamp;
        }

        internal class OrderBookEntryResponse
        {
            public decimal price;
            public decimal amount;
            public decimal timestamp;
        }

        internal class OrderBookResponse
        {
            public OrderBookEntryResponse[] bids;
            public OrderBookEntryResponse[] asks;
        }

        #endregion
    }
}
