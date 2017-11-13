using Prime.Common;
using System;
using System.Linq;
using System.Web.UI;
using LiteDB;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public sealed class RouteStage : IUniqueIdentifier<ObjectId>
        {
            public readonly ExchangeHop ExchangeHop;
            public readonly Asset AssetTransfer;
            public readonly Asset AssetFinal;
            public readonly AssetPair DestinationTradePair;
            public readonly ObjectId DestinationHash;
            public readonly ObjectId DestinationReversedHash;

            public RouteStage(ExchangeHop hop, Asset assetTransfer)
            {
                ExchangeHop = hop;
                AssetTransfer = assetTransfer;
                AssetFinal = hop.Pair.Other(AssetTransfer);
                DestinationTradePair = new AssetPair(assetTransfer, AssetFinal);
                Id = (DestinationTradePair + ":" + hop.NetworkLow.Id + ":" + hop.NetworkHigh.Id).GetObjectIdHashCode();
                DestinationHash = (ExchangeHop.NetworkHigh.Id + AssetFinal.ShortCode).GetObjectIdHashCode();
                DestinationReversedHash = (ExchangeHop.NetworkLow.Id + AssetTransfer.ShortCode).GetObjectIdHashCode();
            }

            public decimal GetPercentChange(decimal percent = 100)
            {
                var perc = ExchangeHop.Percentage / 100;
                return percent * perc;
            }

            public Money Calculate(bool reversed, Money balance)
            {
                var mult = TradeMultiplier(reversed);
                if (reversed)
                    return new Money(balance.ToDecimalValue() * mult, AssetTransfer);

                return new Money(balance.ToDecimalValue() * mult, AssetFinal);
            }

            public decimal TradeMultiplier(bool reversed)
            {
                return reversed ? ExchangeHop.Low.AsQuote(AssetFinal).Price.ToDecimalValue() : ExchangeHop.High.AsQuote(AssetTransfer).Price.ToDecimalValue();
            }

            public (string, Money) Explain(int step, bool reversed, Money balance)
            {
                var s = $"{Environment.NewLine} {step}) ";
                if (reversed)
                    s += $"SELL {AssetFinal} @ {ExchangeHop.High.Network.Name} TO {AssetTransfer} -> XFER  -> {ExchangeHop.Low.Network.Name} [-{Math.Round(ExchangeHop.Percentage, 2)}%] ";
                else
                    s += $"From {ExchangeHop.Low.Network.Name} withdraw {AssetTransfer} to {ExchangeHop.High.Network.Name} -> Sell {AssetTransfer} to {AssetFinal} [@ {Math.Round(TradeMultiplier(reversed), 6)}] [+{Math.Round(ExchangeHop.Percentage, 2)}%]";

                var newBalance = Calculate(reversed, balance);
                
                s += " : " + newBalance;
                return (s, newBalance);
            }

            public ObjectId Id { get; }
        }
    }
}