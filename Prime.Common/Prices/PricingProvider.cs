using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Core.Market;
using Prime.Utility;

namespace Prime.Common
{
    public class PricingProvider
    {
        public readonly IAggVolumeDataProvider ProviderAggVolumeData;
        public readonly IReadOnlyList<IPublicPricingProvider> ProvidersPublicPricing;

        public static PricingProvider I => Lazy.Value;
        private static readonly Lazy<PricingProvider> Lazy = new Lazy<PricingProvider>(() => new PricingProvider());

        private PricingProvider()
        {
            ProviderAggVolumeData = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
            ProvidersPublicPricing = Networks.I.Providers.OfType<IPublicPricingProvider>().Where(x => x.IsDirect).ToList();
        }

        public async Task<MarketPrices> GetAsync(IEnumerable<Network> networks, PricingProviderContext context = null)
        {
            var t = networks.Select(x => GetTask(x, context));
            var r = await t.WhenAll().ConfigureAwait(false);
            return new MarketPrices(r);
        }

        public async Task<MarketPrices> GetAsync(Network network, PricingProviderContext context = null)
        {
            var t = GetTask(network, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetAsync(Network network, IEnumerable<AssetPair> pairs, PricingProviderContext context = null)
        {
            var t = GetTask(network, pairs, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public async Task<MarketPrices> GetAsync(Network network, AssetPair pair, PricingProviderContext context = null)
        {
            var t = GetTask(network, pair, context);
            if (t == null)
                return null;
            return await t.ConfigureAwait(false);
        }

        public Task<MarketPrices> GetTask(Network network, PricingProviderContext context = null)
        {
            var provs = network.PublicPriceProviders.Where(x => x.PricingFeatures?.HasBulk == true && x.PricingFeatures.Bulk.CanReturnAll);

            if (context?.RequestVolume == true)
                provs = provs.Where(x => x.PricingFeatures?.Bulk?.CanVolume == true);

            if (context?.RequestStatistics == true)
                provs = provs.Where(x => x.PricingFeatures?.Bulk?.CanStatistics == true);

            var pp = DirectChoiceTask(provs, new PublicPricesContext(), context);
            if (pp !=null)
                return pp;

            if (context?.UseReturnAll == true)
                return default;
            
            async Task<MarketPrices> Pairs()
            {
                var newContext = context?.Clone() ?? new PricingProviderContext();
                newContext.UseReturnAll = false;

                var pairs = await AssetPairProvider.I.GetPairsAsync(network).ConfigureAwait(false);
                if (pairs == null)
                    return default;
                return await GetAsync(network, pairs, newContext).ConfigureAwait(false);
            }

            return Pairs();
        }

        public Task<MarketPrices> GetTask(Network network, IEnumerable<AssetPair> pairs, PricingProviderContext context = null)
        {
            if (pairs == null)
                return default;

            if (pairs.Count() > 1 && context?.UseReturnAll != false)
                return GetTask(network, context);

            var provs = network.PublicPriceProviders.Where(x=>x.PricingFeatures?.HasBulk == true).AsEnumerable();

            if (context?.RequestVolume == true)
                provs = provs.Where(x => x.PricingFeatures?.Bulk?.CanVolume == true);

            if (context?.RequestStatistics == true)
                provs = provs.Where(x => x.PricingFeatures?.Bulk?.CanStatistics == true);

            var task = DirectChoiceTask(provs, new PublicPricesContext(pairs.AsList()), context);
            if (task != null)
                return task;

            if (context?.OnlyBulk == true)
                return default;

            var singles = pairs.Select(pair => GetTask(network, pair, context)).Where(x => x != null);
            return singles.WhenAll().ContinueWith(r => r?.Result == null ? new MarketPrices() : new MarketPrices(r.Result));
        }

        public Task<MarketPrices> GetTask(Network network, AssetPair pair, PricingProviderContext context = null)
        {
            var provs = network.PublicPriceProviders.AsEnumerable();

            if (context?.RequestVolume == true)
                provs = provs.Where(x => x.PricingFeatures?.HasVolume == true);

            if (context?.RequestStatistics == true)
                provs = provs.Where(x => x.PricingFeatures?.HasStatistics == true);

            var task = DirectChoiceTask(provs, new PublicPriceContext(pair), context);
            if (task != null)
                return task;

            return default;
        }

        private Task<MarketPrices> DirectChoiceTask(IEnumerable<IPublicPricingProvider> provs, PublicPricesContext pContext, PricingProviderContext context = null)
        {
            if (!provs.Any())
                return default;

            if (context?.UseDirect != false)
            {
                var pd = provs.FirstOrDefault(x => x.IsDirect);
                if (pd != null)
                    return CoordinatorTask(pd, pContext, context);
            }

            if (context?.UseDirect != true)
            {
                var pa = provs.FirstOrDefault(x => !x.IsDirect);
                if (pa != null)
                    return CoordinatorTask(pa, pContext, context);
            }

            return default;
        }

        private Task<MarketPrices> CoordinatorTask(IPublicPricingProvider prov, PublicPricesContext pContext, PricingProviderContext context = null)
        {
            return ApiCoordinator.GetPricingAsync(prov, pContext).ContinueWith(x =>
            {
                var r = x.Result;
                return r.IsNull ? null : After(r.Response, context);
            });
        }

        private MarketPrices After(MarketPrices response, PricingProviderContext context)
        {
            if (response == null || context?.AfterData == null)
                return response;

            context?.AfterData?.Invoke(response);
            return response;
        }
    }
}