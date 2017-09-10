using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins.Services.Coinbase
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

        internal class Pagination
        {
            public string ending_before;
            public string starting_after;
            public string limit;
            public string order;
            public string previous_uri;
            public string next_uri;
        }

        internal class Accounts
        {
            public Pagination pagination;
            public List<Account> data;
        }

        internal class Account : BaseDocument
        {
            public string primary;
            public string type;
            public string currency;
            public Balance balance;
            public Balance native_balance;
        }


        internal class Balance
        {
            public decimal amount;
            public string currency;
        }

        internal class WalletAddresses
        {
            public Pagination pagination;
            public List<WalletAddress> data;
        }

        internal class WalletAddress : BaseDocument
        {
            public string address;
            public string network;
        }

        internal class CreateWalletAddress
        {
            public List<WalletAddress> data;
        }
    }
}
