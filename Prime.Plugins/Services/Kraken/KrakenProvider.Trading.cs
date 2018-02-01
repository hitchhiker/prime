using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.Kraken
{
    public partial class KrakenProvider : IBalanceProvider, IDepositProvider
    {
        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body).ConfigureAwait(false);

            CheckResponseErrors(r);

            var results = new BalanceResults(this);

            foreach (var pair in r.result)
            {
                var asset = pair.Key.ToAsset(this);
                results.Add(asset, pair.Value, 0);
            }

            return results;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var assets = await GetAssetPairsAsync(context).ConfigureAwait(false);

            var addresses = new WalletAddresses();

            foreach (var pair in assets)
            {
                var fundingMethod = await GetFundingMethodAsync(context, pair.Asset1).ConfigureAwait(false);

                if (fundingMethod == null)
                    throw new NullReferenceException("No funding method is found");

                var localAddresses = await GetAddressesLocalAsync(api, fundingMethod, pair.Asset1).ConfigureAwait(false);

                addresses.AddRange(localAddresses);
            }

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var fundingMethod = await GetFundingMethodAsync(context, context.Asset).ConfigureAwait(false);

            if (fundingMethod == null)
                throw new NullReferenceException("No funding method is found");

            var addresses = await GetAddressesLocalAsync(api, fundingMethod, context.Asset).ConfigureAwait(false);

            return addresses;
        }
    }
}
