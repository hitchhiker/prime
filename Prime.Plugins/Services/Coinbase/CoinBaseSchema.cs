using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.Coinbase
{
    internal class CoinbaseSchema
    {
        internal class BaseResponse<T>
        {
            public T data;
        }

        internal class ErrorResponse
        {
            public List<ErrorEntryResponse> errors;
        }

        internal class ErrorEntryResponse
        {
            public string id;
            public string message;
        }

        internal class PaginationResponse
        {
            public string ending_before;
            public string starting_after;
            public string limit;
            public string order;
            public string previous_uri;
            public string next_uri;
        }

        #region Public

        internal class TimeResponse : BaseResponse<TimeDataResponse> { }

        internal class TimeDataResponse
        {
            public DateTime iso;
            public long epoch;
        }

        internal class SpotPriceResponse : BaseResponse<PriceDataResponse> { }

        internal class PriceDataResponse
        {
            public decimal amount;
            public string currency;
        }

        #endregion

        #region Private

        internal class PaymentMethods : BaseResponse<List<PaymentMethod>>
        {
            public PaginationResponse pagination;
        }

        internal class PaymentMethod
        {
            public string id;
            public string type;
            public string name;
            public string currency;
            public bool primary_buy;
            public bool primary_sell;
            public bool allow_buy;
            public bool allow_sell;
            public bool allow_deposit;
            public bool allow_withdraw;
            public bool instant_buy;
            public bool instant_sell;
            public DateTime created_at;
            public DateTime updated_at;
            public string resource;
            public string resource_path;
            public ResourseObjectResponse fiat_account;
        }

        internal class PlaceOrderResponse : BaseResponse<OrderResponse> { }

        internal class OrdersListResponse : BaseResponse<List<OrderResponse>>
        {
            public PaginationResponse pagination;
        }

        internal class OrderResponse
        {
            public string id;
            public string status;
            public PaymentMethodResponse payment_method;
            public TransactionResponse transaction;
            public PriceResponse amount;
            public PriceResponse total;
            public PriceResponse subtotal;
            public DateTime created_at;
            public DateTime updated_at;
            public string resource;
            public string resource_path;
            public bool committed;
            public bool instant;
            public PriceResponse fee;
            public DateTime payout_at;
        }

        internal class PriceResponse
        {
            public decimal amount;
            public string currency;
        }

        internal class ResourseObjectResponse
        {
            public string id;
            public string resource;
            public string resource_path;
        }

        internal class PaymentMethodResponse : ResourseObjectResponse { }

        internal class TransactionResponse : ResourseObjectResponse { }

        internal class BaseDocument
        {
            public string id;
            public string name;
            public DateTime created_at;
            public DateTime updated_at;
            public string resource;
            public string resource_path;
        }


        internal class AccountsResponse : BaseResponse<List<AccountResponse>>
        {
            public PaginationResponse pagination;
        }

        internal class AccountResponse : BaseDocument
        {
            public string primary;
            public string type;
            public string currency;
            public BalanceResponse balance;
            public BalanceResponse native_balance;
        }

        internal class BalanceResponse
        {
            public decimal amount;
            public string currency;
        }

        internal class WalletAddressesResponse : BaseResponse<List<WalletAddressResponse>>
        {
            public PaginationResponse pagination;
        }

        internal class WalletAddressResponse : BaseDocument
        {
            public string address;
            public string network;
        }

        internal class CreateWalletAddressResponse : BaseResponse<List<WalletAddressResponse>> { }

        #endregion
    }
}
