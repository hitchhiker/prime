using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Jojatekok.PoloniexAPI;
using KrakenApi;
using LiteDB;
using Prime.Utility;
using AssetPair = Prime.Core.AssetPair;

namespace plugins
{
    public class KrakenProvider : IExchangeProvider, IWalletService
    {
        public Network Network { get; } = new Network("Kraken");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(ApiKey key = null) where T : class
        {
            if (key==null)
                return new KrakenApi.Kraken() as T;

            return new KrakenApi.Kraken(key.Key, key.Secret) as T;
        }

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return new KrakenApi.Kraken() as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            return new KrakenApi.Kraken(key.Key, key.Secret) as T;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public Task<bool> TestApiAsync(ApiTestContext context)
        {
            var t = new Task<bool>(() =>
            {
                var api = GetApi<Kraken>(context);
                var r = api.GetAccountBalance();
                return r != null;
            });
            t.Start();
            return t;
        }

        private static readonly ObjectId IdHash = "prime:kraken".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        public Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var t = new Task<LatestPrice>(() =>
            {
                var kraken = GetApi<Kraken>();
                var ticker = context.Pair.TickerSimple();
                var result = kraken.GetOHLC(ticker, 1440);
                var i = result.Pairs.FirstOrDefault(x => x.Key == ticker);
                var m = new Money(i.Value.OrderBy(x => x.Time).Last().Open, context.Pair.Asset1);
                return new LatestPrice(m);
            });

            t.RunSynchronously();
            return t;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() =>
            {
                var kraken = GetApi<Kraken>(context);
                var d = kraken.GetAssetPairs();
                var aps = new AssetPairs();
                foreach (var assetPair in d)
                {
                    var ticker = assetPair.Key;
                    var first = assetPair.Value.Base;
                    var second = ticker.Replace(first, "");
                    aps.Add(new AssetPair(first, second, this));
                }
                return aps;
            });

            t.RunSynchronously();
            return t;
        }

        public bool CanMultiDepositAddress { get; }
        public bool CanGenerateDepositAddress { get; }

        public Task<WalletAddresses> FetchAllDepositAddressesAsync(WalletAddressContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var t = new Task<BalanceResults>(() =>
            {
                var kraken = GetApi<Kraken>(context);
                var d = kraken.GetAccountBalance();
                var results = new BalanceResults(this);

                foreach (var kv in d)
                    results.AddAvailable(kv.Key.ToAsset(this), kv.Value);

                return results;
            });
            t.Start();
            return t;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return KrakenCodeConverterBase.I;
        }

        public string GetFundingMethod(NetworkProviderPrivateContext context, Asset asset)
        {
            var kraken = GetApi<Kraken>(context);
            var d = kraken.GetDepositMethods(null, asset.ToRemoteCode(this));
            if (d == null || d.Length == 0)
                return null;

            return d[0].Method;
        }

        public Task<WalletAddresses> GetDepositAddressesAsync(WalletAddressAssetContext context)
        {
            var t = new Task<WalletAddresses>(() =>
            {
                var asset = context.Asset;
                var fm = GetFundingMethod(context, asset);
                if (fm == null)
                    return null;

                var addresses = new WalletAddresses();
                var kraken = this.GetApi<Kraken>(context);
                var d = kraken.GetDepositAddresses(asset.ToRemoteCode(this), fm, null, context.CanGenerateAddress);

                foreach (var r in d)
                {
                    if (string.IsNullOrWhiteSpace(r.Address))
                        continue;
                    addresses.Add(new WalletAddress(this, asset) {Address = r.Address, Tag = r.Tag});
                }

                return addresses;
            });
            t.Start();
            return t;
        }

        public OhclData GetOhlc(AssetPair pair, TimeResolution market)
        {
            return null;
        }
    }
}