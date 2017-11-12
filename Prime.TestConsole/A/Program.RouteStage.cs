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

            public RouteStage(ExchangeHop hop, Asset assetTransfer)
            {
                ExchangeHop = hop;
                AssetTransfer = assetTransfer;
                AssetFinal = hop.Pair.Other(AssetTransfer);
                DestinationTradePair = new AssetPair(assetTransfer, AssetFinal);
                Id = (DestinationTradePair + ":" + hop.NetworkLow.Id + ":" + hop.NetworkHigh.Id).GetObjectIdHashCode();
            }

            public decimal GetPercentChange(decimal percent = 100)
            {
                var perc = ExchangeHop.Percentage / 100;
                return percent * perc;
            }

            public string Explain(int step)
            {
                var s = $"{Environment.NewLine} {step}) ";
                s += $"{AssetTransfer} @ {ExchangeHop.Low.Network.Name} [{Math.Round(ExchangeHop.Percentage, 2)}%] {AssetFinal} @ {ExchangeHop.High.Network.Name}";
                return s;
            }

            public ObjectId Id { get; }
        }
    }
}