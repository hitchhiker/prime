using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairDiscoveries : IHasProcessState
    {
        private const string IntermediariesCsv = "USD,BTC,EUR,LTC,USDT,XRP,ETH,ETC,BCC,BCH";
        private static readonly List<Asset> Intermediaries = IntermediariesCsv.ToCsv().Select(x => x.ToAssetRaw()).ToList();
        public readonly AssetPairDiscoveryRequestMessage Context;
        public readonly List<AssetPairNetworks> Discovered = new List<AssetPairNetworks>();

        private readonly object _lock = new object();

        public ProcessState ProcessState { get; private set; }
        public AssetPairNetworks DiscoverFirst { get; private set; }

        internal AssetPairDiscoveries(AssetPairDiscoveryRequestMessage requestMessage)
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
            DiscoverFirst = DiscoverSpecified(pair) ??
                       Discover(pair, Context.ReversalEnabled) ??
                       DiscoverPegged(pair, Context.ReversalEnabled, Context.PeggedEnabled) ??
                       DiscoverIntermediary(pair);
        }

        private AssetPairNetworks DiscoverSpecified(AssetPair pair)
        {
            if (Context.Network == null)
                return null;

            return DiscoverSpecified(pair, Context.Network) ?? (Context.ReversalEnabled ? DiscoverSpecified(pair.Reverse(), Context.Network) : null);
        }

        private static AssetPairNetworks DiscoverSpecified(AssetPair pair, Network network)
        {
            var provs = GetProviders(pair).Where(x => x.Equals(network)).ToList();
            return provs.Any() ? new AssetPairNetworks(pair, provs) : null;
        }

        private static AssetPairNetworks Discover(AssetPair pair, bool canReverse)
        {
            return DiscoverReversable(pair) ?? (canReverse ? DiscoverReversable(pair.Reverse()) : null);
        }

        private static AssetPairNetworks DiscoverReversable(AssetPair pair)
        {
            var provs = GetProviders(pair);
            return provs.Any() ? new AssetPairNetworks(pair, provs) : null;
        }

        private static AssetPairNetworks DiscoverPegged(AssetPair pair, bool canReverse, bool canPeg)
        {
            if (!canPeg)
                return null;

            return DiscoverPeggedReversable(pair) ?? (canReverse ? DiscoverPeggedReversable(pair.Reverse()) : null);
        }

        private static AssetPairNetworks DiscoverPeggedReversable(AssetPair pair)
        {
            // Try alternate / pegged variation

            foreach (var ap in pair.PeggedPairs)
            {
                var provs = GetProviders(ap);
                if (provs.Any())
                    return new AssetPairNetworks(ap, provs) {IsPegged = true};
            }

            return null;
        }
        
        private AssetPairNetworks DiscoverIntermediary(AssetPair pair)
        {
            if (!Context.ConversionEnabled)
                return null;

            return DiscoverIntermediariesLoop(pair);
        }

        private AssetPairNetworks DiscoverIntermediariesLoop(AssetPair pair)
        {
            var ints = Intermediaries.Select(intermediary => DoIntermerdiaryReverseable(pair, intermediary)).Where(provs => provs != null).ToList();
            return ints.OrderByDescending(x=>x.Sort).ThenByDescending(x=>x.TotalNetworksInvolved).FirstOrDefault();
        }

        private AssetPairNetworks DoIntermerdiaryReverseable(AssetPair pair, Asset intermediary)
        {
            var p1 = DiscoverFromIntermediary(pair, intermediary, Context.PeggedEnabled);
            var p2 = Context.ReversalEnabled ? DiscoverFromIntermediary(pair.Reverse(), intermediary, Context.PeggedEnabled) : null;

            if (p1 != null)
                Discovered.Add(p1);

            if (p2 != null)
                Discovered.Add(p2);

            return p1 ?? p2;
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

            provs1.IsConversionPart1 = true;
            provs1.ConversionOther = provs2;

            provs2.IsConversionPart2 = true;
            provs2.ConversionOther = provs1;

            provs1.Intermediary = provs2.Intermediary = intermediary;

            return provs1;
        }

        private static IReadOnlyList<Network> GetProviders(AssetPair pair)
        {
            return AsyncContext.Run(()=> AssetPairProvider.I.GetNetworksAsync(pair, true)).ToList();
        }
    }
}