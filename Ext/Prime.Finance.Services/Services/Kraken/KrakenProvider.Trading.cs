using System;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance.Services.Services.Kraken
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
        
        private async Task<WalletAddressesResult> GetAddressesLocalAsync(IKrakenApi api, string fundingMethod, Asset asset, bool generateNew = false)
        {
            var body = CreateKrakenBody();

            // BUG: do we need "aclass"?
            //body.Add("aclass", asset.ToRemoteCode(this));
            body.Add("asset", asset.ToRemoteCode(this));
            body.Add("method", fundingMethod);
            body.Add("new", generateNew);

            var r = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);
            CheckResponseErrors(r);

            var walletAddresses = new WalletAddressesResult();

            foreach (var addr in r.result)
            {
                var walletAddress = new WalletAddress(this, asset)
                {
                    Address = addr.address
                };

                if (addr.expiretm != 0)
                {
                    var time = addr.expiretm.ToUtcDateTime();
                    walletAddress.ExpiresUtc = time;
                }

                walletAddresses.Add(new WalletAddress(this, asset) { Address = addr.address });
            }

            return walletAddresses;
        }

        public async Task<WalletAddressesResult> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var assets = await GetAssetPairsAsync(context).ConfigureAwait(false);

            var addresses = new WalletAddressesResult();

            foreach (var pair in assets)
            {
                var fundingMethod = await GetFundingMethodAsync(context, pair.Asset1).ConfigureAwait(false);

                if (fundingMethod == null)
                    throw new NullReferenceException("No funding method is found");

                var localAddresses = await GetAddressesLocalAsync(api, fundingMethod, pair.Asset1).ConfigureAwait(false);

                addresses.AddRange(localAddresses.WalletAddresses);
            }

            return addresses;
        }

        public async Task<WalletAddressesResult> GetAddressesForAssetAsync(WalletAddressAssetContext context)
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
