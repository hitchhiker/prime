using System;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class ExchangeHop
        {
            public ExchangeHop(AssetPair pair, MarketPrice price1, MarketPrice price2)
            {
                Pair = pair;

                var direction = price1.Price > price2.Price;

                Low = direction ? price2 : price1;
                High = direction ? price1 : price2;

                var perc = 1 / Low.Price.ToDecimalValue();
                perc = perc * High.Price.ToDecimalValue();
                Percentage = (perc * 100) - 100;

                AssetTransfer = pair.Other(Low.QuoteAsset);
            }

            public readonly AssetPair Pair;
            public readonly Asset AssetTransfer;

            public readonly MarketPrice Low;
            public readonly MarketPrice High;
            public readonly decimal Percentage;

            public Network NetworkLow => Low.Network;

            public Network NetworkHigh => High.Network;

            public bool ForAsset(Asset asset)
            {
                return Pair.Has(asset);
            }

            public override string ToString()
            {
                return $"{Pair} {Low.Network} [{Low.Price}] to {High.Network} [{High.Price}] = {Math.Round(Percentage, 2)}%";
            }

            public string Explain()
            {
                return $"{Pair} @ {Low.Network} to {High.Network} [{Math.Round(Percentage, 2)}%]";
            }


            public ObjectId Id { get; }
        }
    }
}