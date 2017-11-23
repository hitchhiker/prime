using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bisq
{
    internal class BisqSchema
    {
        internal class TickerResponseEntry
        {
            public decimal? last;
            public decimal? high;
            public decimal? low;
            public decimal? volume_left;
            public decimal? volume_right;
            public decimal? buy;
            public decimal? sell;
        }
    }
}
