using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.LakeBtc
{
    internal class LakeBtcSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal? high;
            public decimal? volume;
            public decimal? low;
            public decimal? bid;
            public decimal? ask;
        }
    }
}
