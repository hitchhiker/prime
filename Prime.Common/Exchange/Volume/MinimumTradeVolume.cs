using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class MinimumTradeVolume
    {
        public MinimumTradeVolume()
        {
            Market = AssetPair.Empty;
            MinimumBuy = Money.Zero;
            MinimumSell = Money.Zero;
        }

        public MinimumTradeVolume(AssetPair market) : this()
        {
            Market = market;
        }

        public MinimumTradeVolume(AssetPair market, Money minimumBuy, Money minimumSell) : this(market)
        {
            MinimumBuy = minimumBuy;
            MinimumSell = minimumSell;
        }

        public AssetPair Market { get; }

        public Money MinimumBuy { get; set; }
        public Money MinimumSell { get; set; }

        public bool HasMinimumBase => MinimumBuy != Money.Zero;
        public bool HasMinimumQuote => MinimumSell != Money.Zero;
    }
}
