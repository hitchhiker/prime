using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Korbit
{
    internal class KorbitSchema
    {
        internal class TickerResponse
        {
            public long timestamp;
            public decimal last;
        }
    }
}
