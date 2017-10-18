using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Core;
using Prime.Utility;

namespace Prime.Plugins.Services.Binance
{
    public class BinanceProvider : IExchangeProvider, IOrderBookProvider, IWalletService, IOhlcProvider
    {
        public const string BinanceApiVersion = "v1";
        public const string BinanceApiUrl = "https://www.binance.com/api/" + BinanceApiVersion;

        private static readonly ObjectId IdHash = "prime:bitflyer".GetObjectIdHashCode();

        private RestApiClientProvider<IBinanceApi> ApiProvider { get; }

        public ObjectId Id => IdHash;
        public Network Network { get; } = new Network("Binance");
        public bool Disabled { get; } = false;
        public int Priority => 100;
        public string AggregatorName { get; } = null;
        public string Title => Network.Name;

        private static readonly IRateLimiter Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;
        public bool CanMultiDepositAddress { get; } = false;
        public bool CanGenerateDepositAddress { get; } = false;

        public ApiConfiguration GetApiConfiguration { get; } = ApiConfiguration.Standard2;

        public BinanceProvider()
        {
            ApiProvider = new RestApiClientProvider<IBinanceApi>(BinanceApiUrl, this, k => new BinanceAuthenticator(k).GetRequestModifier);
        }

        public Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            throw new NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetSymbolPriceTicker();

            var lowerPairTicker = context.Pair.TickerSimple().ToLower();

            var lpr = r.FirstOrDefault(x => x.symbol.ToLower().Equals(lowerPairTicker));

            if(lpr == null)
                throw new ApiResponseException("Specified currency pair is not supported by provider", this);

            var latestPrice = new LatestPrice()
            {
                Price = new Money(lpr.price, context.Pair.Asset2),
                BaseAsset = context.Pair.Asset1,
                UtcCreated = DateTime.UtcNow
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var moneyList = new List<Money>();

            var r = await api.GetSymbolPriceTicker();

            foreach (var asset in context.Assets)
            {
                var currentAssetPair = new AssetPair(context.BaseAsset, asset);
                var rPrice = r.FirstOrDefault(x => x.symbol.Equals(currentAssetPair.TickerSimple()));

                if (rPrice == null)
                    continue;

                moneyList.Add(new Money(rPrice.price, asset));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                Prices = moneyList,
                UtcCreated = DateTime.UtcNow
            };

            return latestPrices;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetSymbolPriceTicker();

            var assetPairs = new AssetPairs();

            foreach (var rPrice in r)
            {
                var pair = GetAssetPair(rPrice.symbol, assetPairs);

                if (pair == null)
                    continue;

                assetPairs.Add(pair);
            }

            return assetPairs;
        }

        private AssetPair GetAssetPair(string pairCode, AssetPairs existingPairs)
        {
            AssetPair assetPair = null;

            if (pairCode.Length == 6)
            {
                var asset1 = pairCode.Substring(0, 3);
                var asset2 = pairCode.Substring(3);

                assetPair = new AssetPair(asset1, asset2);
            }
            else if(pairCode.Length > 6)
            {
                var existingAsset = existingPairs.FirstOrDefault(x => pairCode.Contains(x.Asset1.ShortCode))?.Asset1;
                if (existingAsset != null)
                {
                    var asset1 = existingAsset.ShortCode;
                    var asset2 = pairCode.Replace(existingAsset.ShortCode, "");

                    assetPair = new AssetPair(asset1, asset2);
                }
                else
                {
                    existingAsset = existingPairs.FirstOrDefault(x => pairCode.Contains(x.Asset2.ShortCode))?.Asset2;
                    if (existingAsset != null)
                    {
                        var asset1 = pairCode.Replace(existingAsset.ShortCode, "");
                        var asset2 = existingAsset.ShortCode;

                        assetPair = new AssetPair(asset1, asset2);
                    }
                }
            }

            return assetPair;
        }

        public Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            throw new NotImplementedException();
        }

        public Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestApiAsync(ApiTestContext context)
        {
            throw new NotImplementedException();
        }

        public Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
