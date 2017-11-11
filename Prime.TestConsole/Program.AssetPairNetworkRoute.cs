using System;
using Prime.Common;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class AssetPairNetworkRoute
        {
            public AssetPairNetworkRoute(AssetPair pair, MarketPrice price1, MarketPrice price2)
            {
                Pair = pair;
                Low = price1.Price > price2.Price ? price2 : price1;
                High = price1.Price > price2.Price ? price1 : price2;

                var perc = 1 / Low.Price.ToDecimalValue();
                perc = perc * High.Price.ToDecimalValue();
                Percentage = (perc * 100) - 100;
            }

            public readonly AssetPair Pair;
            public readonly MarketPrice Low;
            public readonly MarketPrice High;
            public readonly decimal Percentage;

            public Network NetworkLow => Low.Network;

            public Network NetworkHigh => High.Network;

            public override string ToString()
            {
                return $"{Pair} {Low.Network} [{Low.Price}] to {High.Network} [{High.Price}] = {Math.Round(Percentage, 2)}%";
            }
        }
    }
}