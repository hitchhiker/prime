using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bisq
{
    internal class BisqSchema
    {
        internal class BaseResponse<T>
        {
            public T result;
        }

        internal class TickerResponse : BaseResponse<Dictionary<string, TickerResponseEntry>> { }

        internal class TickerResponseEntry
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal volume_left;
            public decimal volume_right;
            public decimal buy;
            public decimal sell;
        }
    }
}
