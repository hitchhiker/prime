using System;

namespace Prime.Common
{
    public class PricingProviderContext
    {
        public Func<MarketPrices, MarketPrices> AfterData { get; set; }

        public bool? UseReturnAll { get; set; }

        public bool? UseDirect { get; set; }

        public bool OnlyBulk { get; set; }

        public bool RequestVolume { get; set; }

        public bool RequestStatistics { get; set; }

        public PricingProviderContext Clone()
        {
            return new PricingProviderContext()
            {
                AfterData = AfterData,
                UseReturnAll = UseReturnAll,
                OnlyBulk = OnlyBulk,
                UseDirect = UseDirect,
                RequestVolume = RequestVolume,
                RequestStatistics = RequestStatistics
            };
        }
    }
}