using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.Coinbase
{
    internal class CoinbaseSchema
    {
        internal class BaseDocument
        {
            public string id;
            public string name;
            public DateTime created_at;
            public DateTime updated_at;
            public string resource;
            public string resource_path;
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

        internal class TimeResponse
        {
            public TimeDataResponse data;
        }

        internal class TimeDataResponse
        {
            public DateTime iso;
            public long epoch;
        }

        internal class SpotPriceResponse
        {
            public PriceDataResponse data;
        }

        internal class PriceDataResponse
        {
            public decimal amount;
            public string currency;
        }

        internal class AccountsResponse
        {
            public PaginationResponse pagination;
            public List<AccountResponse> data;
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

        internal class WalletAddressesResponse
        {
            public PaginationResponse pagination;
            public List<WalletAddressResponse> data;
        }

        internal class WalletAddressResponse : BaseDocument
        {
            public string address;
            public string network;
        }

        internal class CreateWalletAddressResponse
        {
            public List<WalletAddressResponse> data;
        }
    }
}
