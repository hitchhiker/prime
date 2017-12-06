using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Exx
{
    internal class ExxSchema
    {
        internal class TickerResponse
        {
            public TickerEntry ticker;
            public long date;
        }

        internal class TickerEntry
        {
            public float vol;
            public decimal last;
            public decimal sell;
            public decimal buy;
            public decimal weekRiseRate;
            public decimal riseRate;
            public decimal high;
            public decimal low;
            public decimal monthRiseRate;
        }
    }
}
