using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Finance
{
    public class MinimumTradeVolume
    {
        public MinimumTradeVolume()
        {

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

        public AssetPair Market { get; } = AssetPair.Empty;

        public Money MinimumBuy { get; set; } = Money.Zero;
        public Money MinimumSell { get; set; } = Money.Zero;

        public bool HasMinimumBase => MinimumBuy != Money.Zero;
        public bool HasMinimumQuote => MinimumSell != Money.Zero;
    }
}
