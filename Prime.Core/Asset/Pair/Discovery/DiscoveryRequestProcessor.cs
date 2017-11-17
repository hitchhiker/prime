using System;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Core.Prices.Latest
{
    internal sealed class DiscoveryRequestProcessor
    {
        private readonly Request _req;
        private readonly Action _doAfterDiscovery;

        internal DiscoveryRequestProcessor(Request req, Action doAfterDiscovery = null)
        {
            _req = req;
            _doAfterDiscovery = doAfterDiscovery;
            new Task(Discover).Start();
        }

        private void Discover()
        {
            var request = new AssetPairDiscoveryRequestMessage
            {
                Network = _req.NetworkSuggested,
                Pair = _req.Pair,
                ConversionEnabled = true,
                PeggedEnabled = true,
                ReversalEnabled = true
            };

            var r = AssetPairDiscovery.I.Discover(request);

            if (r == null)
                return;

            if (!r.DiscoverFirst.Has<IPublicPricingProvider>())
                return;

            ProcessDiscovery(_req, r.DiscoverFirst);

            _doAfterDiscovery?.Invoke();
        }

        private static void ProcessDiscovery(Request request, AssetPairNetworks r, bool isPart2 = false)
        {
            request.IsConvertedPart1 = !isPart2 && r.ConversionPart2 != null;
            request.Discovered = r;
            request.IsDiscovered = true;

            if (request.IsConvertedPart1 && !isPart2)
                ProcessConverted(request, r.ConversionPart2);
        }

        private static void ProcessConverted(Request request, AssetPairNetworks networks)
        {
            var part2 = new Request(request.Pair, networks.Network<IPublicPricingProvider>())
            {
                IsConvertedPart1 = false,
                IsConvertedPart2 = true
            };

            ProcessDiscovery(part2, networks, true);
        }
    }
}