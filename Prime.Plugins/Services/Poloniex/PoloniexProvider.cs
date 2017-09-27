using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using LiteDB;
using Prime.Core;
using Prime.Plugins.Services.Base;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Poloniex
{
    public class PoloniexProvider : IExchangeProvider, IWalletService, IOhlcProvider, IApiProvider
    {
        private const String PoloniexApiUrl = "https://poloniex.com";

        public Network Network { get; } = new Network("Poloniex");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:poloniex".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IPoloniexApi>(PoloniexApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);

            return RestClient.For<IPoloniexApi>(PoloniexApiUrl, new PoloniexAuthenticator(key).GetRequestModifier) as T;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IPoloniexApi>(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnBalances);

            try
            {
                var r = await api.GetBalancesAsync(body);

                return r != null && r.Count > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IPoloniexApi>(context);

            var r = await api.GetTickerAsync();

            var assetPairsInfo = r.Where(x => x.Key.ToAssetPair(this).Equals(context.Pair)).ToList();

            if (assetPairsInfo.Count < 1)
                throw new ApiResponseException("Specified asset pair is not supported by this API", this);

            var selectedPair = assetPairsInfo[0];

            var price = new LatestPrice()
            {
                BaseAsset = context.Pair.Asset1,
                Price = new Money(selectedPair.Value.last)
            };

            return price;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = GetApi<IPoloniexApi>(context);

            var r = await api.GetTickerAsync();

            var pairs = new AssetPairs();

            foreach (var rPair in r)
            {
                var pair = rPair.Key.ToAssetPair(this);

                pairs.Add(pair);
            }

            return pairs;
        }

        public bool CanMultiDepositAddress { get; } = true;
        public bool CanGenerateDepositAddress { get; } = true;

        public async Task<WalletAddresses> FetchAllDepositAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = GetApi<IPoloniexApi>(context);

            var body = CreatePoloniexBody(PoloniexBodyType.ReturnCompleteBalances);

            var r = await api.GetBalancesDetailedAsync(body);

            var results = new BalanceResults(this);

            foreach (var kvp in r)
            {
                var c = kvp.Key.ToAsset(this);

                results.Add(new BalanceResult(c)
                {
                    Available = kvp.Value.available,
                    Reserved = kvp.Value.onOrders,
                    Balance = kvp.Value.available
                });
            }

            return results;
        }

        private Dictionary<string, object> CreatePoloniexBody(PoloniexBodyType bodyType)
        {
            var body = new Dictionary<string, object>();

            body.Add("nonce", BaseAuthenticator.GetNonce());

            switch (bodyType)
            {
                case PoloniexBodyType.ReturnBalances:
                    body.Add("command", "returnBalances");
                    break;
                case PoloniexBodyType.ReturnCompleteBalances:
                    body.Add("command", "returnCompleteBalances");
                    break;
                case PoloniexBodyType.ReturnDepositAddresses:
                    body.Add("command", "returnDepositAddresses");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
            }

            return body;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<WalletAddresses> GetDepositAddressesAsync(WalletAddressAssetContext context)
        {
            var api = GetApi<IPoloniexApi>(context);
            var body = CreatePoloniexBody(PoloniexBodyType.ReturnDepositAddresses);

            var addresses = new WalletAddresses();

            // TODO: check using verified account.
            try
            {
                var r = await api.GetDepositAddresses(body);
                var assetBalances = r.Where(x => Equals(x.Key.ToAsset(this), context.Asset)).ToArray();

                foreach (var balance in assetBalances)
                {
                    if (string.IsNullOrWhiteSpace(balance.Value))
                        continue;

                    addresses.Add(new WalletAddress(this, balance.Key.ToAsset(this)) { Address = balance.Value });
                }
            }
            catch (Exception e)
            {
                throw new ApiResponseException("Unable to get deposit addresses, please check if your account is verified", this);
            }

            return addresses;
        }

        public async Task<OhclData> GetOhlcAsync(OhlcContext context)
        {
            var pair = context.Pair;
            var market = context.Market;

            var api = GetApi<PoloniexClient>(context);
            var cpair = new CurrencyPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));
            var mp = MarketPeriod.Hours2;
            var ds = DateTime.UtcNow.AddDays(-10);
            var de = DateTime.UtcNow;
            var apir = await api.Markets.GetChartDataAsync(cpair, mp, ds, de);
            var r = new OhclData(market);
            var seriesid = OhlcResolutionAdapter.GetHash(pair, market, Network);
            foreach (var i in apir)
            {
                r.Add(new OhclEntry(seriesid, i.Time, this)
                {
                    Open = i.Open,
                    Close = i.Close,
                    Low = i.Low,
                    High = i.High,
                    VolumeTo = (long)i.VolumeQuote,
                    VolumeFrom = (long)i.VolumeBase,
                    WeightedAverage = i.WeightedAverage
                });
            }
            return r;
        }
    }
}
