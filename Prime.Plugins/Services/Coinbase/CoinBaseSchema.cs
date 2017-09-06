using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins.Services.Coinbase
{
    public class BaseDocument
    {
        public string id;
        public string name;
        public DateTime created_at;
        public DateTime updated_at;
        public string resource;
        public string resource_path;
    }


    public class Pagination
    {
        public string ending_before;
        public string starting_after;
        public string limit;
        public string order;
        public string previous_uri;
        public string next_uri;
    }

    public class Accounts
    {
        public Pagination pagination;
        public List<Account> data;
    }

    public class Account : BaseDocument
    {
        public string primary;
        public string type;
        public string currency;
        public Balance balance;
        public Balance native_balance;
    }


    public class Balance
    {
        public decimal amount;
        public string currency;
    }

    public class WalletAddresses
    {
        public Pagination pagination;
        public List<WalletAddress> data;
    }

    public class WalletAddress : BaseDocument
    {
        public string address;
        public string network;
    }

    public class CreateWalletAddress
    {
        public List<WalletAddress> data;
    }
}
