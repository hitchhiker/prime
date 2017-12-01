using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common
{
    public class MarketBasePrices
    {
        [Bson]
        public DateTime UtcCreated { get; set; }

        [Bson]
        public Asset BaseAsset { get; set; }

        [Bson]
        public List<Money> Prices { get; set; } = new List<Money>();
    }
}
