using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Paymium
{
    internal class PaymiumSchema
    {
        internal class TickerResponse
        {
            public string currency;
            public decimal? high;
            public decimal? low;
            public decimal? volume;
            public decimal? bid;
            public decimal? ask;
            public decimal? variation;
            public decimal? midpoint;
            public decimal? vwap;
            public decimal price;
            public decimal? open;
            public decimal? size;
            public long at;
        }

        internal class CountriesResponse
        {
            public int id;
            public int? iso_numeric;
            public int? telephone_code;
            public bool accepted;
            public string iso_alpha2;
            public string iso_alpha3;
            public string name_en;
            public string name_fr;
        }
    }
}
