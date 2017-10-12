using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Coinbase
{
    internal class GdaxSchema
    {
        internal class ProductsResponse : List<ProductResponse> { }

        internal class ProductResponse
        {
            public string id;
            public string base_currency;
            public string quote_currency;
            public decimal base_min_size;
            public decimal base_max_size;
            public decimal quote_increment;
        }

        internal class OrderBookResponse
        {
            public string sequence;
            public string[][] bids;
            public string[][] asks;
        }

        internal class InvalidResponse
        {
            public string message;
        }
    }
}
