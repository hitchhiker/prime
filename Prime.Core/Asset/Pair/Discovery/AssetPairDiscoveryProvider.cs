using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairDiscoveryProvider
    {
        private readonly AssetPairDiscoveryRequestMessage _requestMessage;
        private readonly AssetPair _pair;
        private const string IntermediariesCsv = "USD,BTC,EUR,LTC,USDT,XRP,ETH,ETC,BCC,BCH";
        private static readonly List<Asset> Intermediaries = IntermediariesCsv.ToCsv().Select(x => x.ToAssetRaw()).ToList();
        public bool IsFinished;
        private readonly object _lock = new object();
        private AssetPairNetworks _providers;

        internal AssetPairDiscoveryProvider(AssetPairDiscoveryRequestMessage requestMessage)
        {
            _requestMessage = requestMessage;
            _pair = requestMessage.Pair;
            new Task(Discover).Start();
        }

        internal void Discover()
        {
            if (_pair == null)
                return;

            Discover(_pair);

            lock (_lock)
                IsFinished = true;

            SendMessage();
        }

        internal void SendMessage()
        {
            lock (_lock)
            {
                if (!IsFinished)
                    return;
            }

            DefaultMessenger.I.Default.SendAsync(new AssetPairDiscoveryResultMessage(_requestMessage, _providers));
        }

        private void Discover(AssetPair pair)
        {
            _providers = DiscoverSpecified(pair) ??
                             Discover(pair, _requestMessage.ReversalEnabled) ??
                             DiscoverPegged(pair, _requestMessage.ReversalEnabled, _requestMessage.PeggedEnabled) ??
                             DiscoverIntermediary(pair);
        }

        private AssetPairNetworks DiscoverSpecified(AssetPair pair)
        {
            if (_requestMessage.Network == null)
                return null;

            return DiscoverSpecified(pair, _requestMessage.Network, false) ?? (_requestMessage.ReversalEnabled ? DiscoverSpecified(pair.Reverse(), _requestMessage.Network, true) : null);
        }

        private static AssetPairNetworks DiscoverSpecified(AssetPair pair, Network network, bool isReversed)
        {
            var provs = GetProviders(pair).Where(x => x.Equals(network)).ToList();
            return provs.Any() ? new AssetPairNetworks(pair, provs, isReversed) : null;
        }

        private static AssetPairNetworks Discover(AssetPair pair, bool canReverse)
        {
            return DiscoverReversable(pair, false) ?? (canReverse ? DiscoverReversable(pair.Reverse(), true) : null);
        }

        private static AssetPairNetworks DiscoverReversable(AssetPair pair, bool isReversed)
        {
            var provs = GetProviders(pair);
            return provs.Any() ? new AssetPairNetworks(pair, provs, isReversed) : null;
        }

        private static AssetPairNetworks DiscoverPegged(AssetPair pair, bool canReverse, bool canPeg)
        {
            if (!canPeg)
                return null;

            return DiscoverPeggedReversable(pair, false) ?? (canReverse ? DiscoverPeggedReversable(pair.Reverse(), true) : null);
        }

        private static AssetPairNetworks DiscoverPeggedReversable(AssetPair pair, bool isReversed)
        {
            // Try alternate / pegged variation

            foreach (var ap in pair.PeggedPairs)
            {
                var provs = GetProviders(ap);
                if (provs.Any())
                    return new AssetPairNetworks(ap, provs, isReversed) {IsPegged = true};
            }

            return null;
        }
        
        private AssetPairNetworks DiscoverIntermediary(AssetPair pair)
        {
            if (!_requestMessage.ConversionEnabled)
                return null;

            return DiscoverIntermediaries(pair, _requestMessage.PeggedEnabled);
        }

        private static AssetPairNetworks DiscoverIntermediaries(AssetPair pair, bool canPeg)
        {
            return Intermediaries.Select(intermediary => DiscoverFromIntermediary(pair, intermediary, canPeg)).FirstOrDefault(provs => provs != null);
        }

        private static AssetPairNetworks DiscoverFromIntermediary(AssetPair originalPair, Asset intermediary, bool canPeg)
        {
            var pair = new AssetPair(originalPair.Asset1, intermediary);

            var provs1 = Discover(pair, true) ?? DiscoverPegged(pair, true, canPeg);

            if (provs1 == null)
                return null;

            //we have a possible path, find the other side.

            var npair1 = new AssetPair(originalPair.Asset2, intermediary);

            var provs2 = Discover(npair1, true) ?? DiscoverPegged(npair1, true, canPeg);

            if (provs2 == null)
                return null;

            provs1.ConversionPart2 = provs2;
            provs2.ConversionPart1 = provs1;

            return provs1;
        }

        private static IReadOnlyList<Network> GetProviders(AssetPair pair)
        {
            /*var apd = PublicContext.I.PubData.GetAggAssetPairData(pair);
            var provs = apd.Exchanges.Count == 0 ? new List<IPublicPriceSuper>() : apd.AllProviders.OfType<IPublicPriceSuper>().DistinctBy(x => x.Id).ToList();
            if (provs.Any())
                return provs;*/

            return AssetPairProvider.I.GetNetworks(pair, true).ToList();
        }
    }
}