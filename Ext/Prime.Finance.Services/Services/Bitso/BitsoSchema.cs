using System;

namespace Prime.Finance.Services.Services.Bitso
{
    internal class BitsoSchema
    {
        #region Base
        internal class BaseResponse<T>
        {
            public bool success;
            public T payload;
        }
        #endregion

        #region Public
        internal class AllTickerResponse : BaseResponse<TickerEntryResponse[]>
        {

        }

        internal class TickerResponse : BaseResponse<TickerEntryResponse>
        {

        }

        internal class TickerEntryResponse
        {
            public string book;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public DateTime created_at;
        }

        internal class OrderBookEntryResponse
        {
            public long sequence;
            public DateTime updated_at;
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
            public string book;
        }

        internal class OrderBookResponse
        {
            public bool success;
            public OrderBookEntryResponse payload;
        }
        #endregion

        #region Private

        internal class ErrorResponse
        {
            public ErrorEntryResponse error;
        }

        internal class ErrorEntryResponse
        {
            public int code;
            public string message;
        }

        internal class OrdersResponse : BaseResponse<OrderInfoEntryResponse[]>
        {

        }

        internal class UserInfoResponse : BaseResponse<UserInfoEntryResponse>
        {

        }

        internal class OrderInfoResponse : BaseResponse<OrderInfoEntryResponse>
        {

        }

        internal class WithdrawalRequestResponse : BaseResponse<WithdrawalRequestEntryResponse>
        {
            
        }

        internal class WithdrawalRequestEntryResponse
        {
            public string wid;
            public string status;
            public long created_at;
            public string currency;
            public string method;
            public decimal amount;
        }

        internal class OrderInfoEntryResponse
        {
            public string oid;
            public string book;
            public decimal original_amount;
            public decimal unfilled_amount;
            public decimal original_value;
            public long created_at;
            public long updated_at;
            public decimal price;
            public string side;
            public string status;
            public string type;
        }

        internal class UserInfoEntryResponse
        {
            public string client_id;
            public string first_name;
            public string last_name;
            public string status;
            public decimal daily_limit;
            public decimal monthly_limit;
            public decimal daily_remaining;
            public decimal monthly_remaining;
            public string cellphone_number;
            public string cellphone_number_stored;
            public string email_stored;
            public string official_id;
            public string proof_of_residency;
            public string signed_contract;
            public string origin_of_funds;
        }
        #endregion
    }
}
