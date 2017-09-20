using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRate
    {
        public DateTime UtcCreated { get; set; }

        public Network Network { get; set; }

        public Money Price { get; set; }

        public AssetPair Pair { get; set; }
    }
}