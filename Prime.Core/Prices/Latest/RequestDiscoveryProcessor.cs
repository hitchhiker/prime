using System;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Core.Prices.Latest
{
    internal sealed class RequestDiscoveryProcessor
    {
        private readonly Request _req;
        private readonly Action _doAfterDiscovery;

        internal RequestDiscoveryProcessor(Request req, Action doAfterDiscovery = null)
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

            if (!r.Has<IPublicPriceSuper>())
                return;

            ProcessDiscoveryResponse(_req, r);

            _doAfterDiscovery?.Invoke();
        }

        private static void ProcessDiscoveryResponse(Request request, AssetPairNetworks r, bool isPart2 = false)
        {
            request.PairForProvider = r.Pair;
            request.IsConvertedPart1 = !isPart2 && r.ConversionPart2 != null;
            request.Discovered = r;
            request.IsDiscovered = true;

            if (request.IsConvertedPart1 && !isPart2)
                ProcessConvertedPart2(request, r.ConversionPart2);
        }

        private static void ProcessConvertedPart2(Request request, AssetPairNetworks networks)
        {
            var part2 = new Request(request.Pair, networks.Network<IPublicPriceSuper>())
            {
                ConvertedOther = request,
                IsConvertedPart1 = false,
                IsConvertedPart2 = true
            };

            request.ConvertedOther = part2;
            ProcessDiscoveryResponse(part2, networks, true);
        }
    }
}