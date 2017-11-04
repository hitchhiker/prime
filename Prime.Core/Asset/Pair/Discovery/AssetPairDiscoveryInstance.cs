using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairDiscoveryInstance : IHasProcessState
    {
        private const string IntermediariesCsv = "USD,BTC,EUR,LTC,USDT,XRP,ETH,ETC,BCC,BCH";
        private static readonly List<Asset> Intermediaries = IntermediariesCsv.ToCsv().Select(x => x.ToAssetRaw()).ToList();
        public readonly AssetPairDiscoveryRequestMessage Context;

        private readonly object _lock = new object();

        public ProcessState ProcessState { get; private set; }
        public AssetPairNetworks Networks { get; private set; }

        internal AssetPairDiscoveryInstance(AssetPairDiscoveryRequestMessage requestMessage)
        {
            Context = requestMessage;
            ProcessState = ProcessState.None;
            new Task(Discover).Start();
        }
        
        private void Discover()
        {
            lock (_lock)
            {
                if (Context.Pair == null)
                    ProcessState = ProcessState.Failed;
                    
                if (ProcessState != ProcessState.None)
                    return;
            }

            Discover(Context.Pair);

            lock (_lock)
                ProcessState = ProcessState.Success;
        }
        
        private void Discover(AssetPair pair)
        {
            Networks = DiscoverSpecified(pair) ??
                       Discover(pair, Context.ReversalEnabled) ??
                       DiscoverPegged(pair, Context.ReversalEnabled, Context.PeggedEnabled) ??
                       DiscoverIntermediary(pair);
        }

        private AssetPairNetworks DiscoverSpecified(AssetPair pair)
        {
            if (Context.Network == null)
                return null;

            return DiscoverSpecified(pair, Context.Network, false) ?? (Context.ReversalEnabled ? DiscoverSpecified(pair.Reverse(), Context.Network, true) : null);
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
            if (!Context.ConversionEnabled)
                return null;

            return DiscoverIntermediaries(pair, Context.PeggedEnabled);
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
            return AsyncContext.Run(()=> AssetPairProvider.I.GetNetworksAsync(pair, true)).ToList();
        }
    }
}