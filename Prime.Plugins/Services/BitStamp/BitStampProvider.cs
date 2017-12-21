using System;
using System.Linq;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Utility;
using LiteDB;
using Newtonsoft.Json;

namespace Prime.Plugins.Services.BitStamp
{
    // https://www.bitstamp.net/api/
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class BitStampProvider : IBalanceProvider, IDepositProvider, IOrderBookProvider, IAssetPairsProvider, IPublicPricingProvider
    {
        private const string BitStampApiUrl = "https://www.bitstamp.net/api/";
        public const string BitStampApiVersion = "v2";

        private static readonly ObjectId IdHash = "prime:bitstamp".GetObjectIdHashCode();
        private const string PairsCsv = "btcusd,btceur,eurusd,xrpusd,xrpeur,xrpbtc,ltcusd,ltceur,ltcbtc,ethusd,etheur,ethbtc";
        private RestApiClientProvider<IBitStampApi> ApiProvider { get; }

        // Do not make more than 600 requests per 10 minutes or we will ban your IP address.
        // https://www.bitstamp.net/api/
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(600, 10);
        public Network Network { get; } = Networks.I.Get("BitStamp");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => null;

        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;

        private AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(3, PairsCsv, this));

        public ApiConfiguration GetApiConfiguration => new ApiConfiguration()
        {
            HasSecret = true,
            HasExtra = true,
            ApiExtraName = "Customer Number"
        };

        public BitStampProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitStampApi>(BitStampApiUrl, this, (k) => new BitStampAuthenticator(k).GetRequestModifierAsync);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAccountBalancesAsync().ConfigureAwait(false);

            return r != null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync(context.Pair.ToTicker(this).ToLower()).ConfigureAwait(false);

            var price = new MarketPrice(Network, context.Pair, r.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.ask, r.bid, r.low, r.high),
                Volume = new NetworkPairVolume(Network, context.Pair, r.volume)
            };

            return new MarketPrices(price);
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAccountBalancesAsync().ConfigureAwait(false);

            var balances = new BalanceResults(this);

            var btcAsset = "btc".ToAsset(this);
            var usdAsset = "usd".ToAsset(this);
            var eurAsset = "eur".ToAsset(this);

            balances.Add(btcAsset, r.btc_available, r.btc_reserved);
            balances.Add(usdAsset, r.usd_available, r.usd_reserved);
            balances.Add(eurAsset, r.eur_available, r.eur_reserved);

            return balances;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context)
        {
            return Task.FromResult<TransferSuspensions>(null);
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var addresses = new WalletAddresses();
            var wac = new WalletAddressAssetContext("ETH".ToAsset(this), context.UserContext, context.L);
            addresses.AddRange(await GetAddressesForAssetAsync(wac).ConfigureAwait(false));

            wac.Asset = "BTC".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac).ConfigureAwait(false));

            wac.Asset = "XRP".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac).ConfigureAwait(false));

            wac.Asset = "LTC".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac).ConfigureAwait(false));

            return addresses;
        }
        
        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);
            var currencyPath = GetCurrencyPath(context.Asset);

            var r = await api.GetDepositAddressAsync(currencyPath).ConfigureAwait(false);

            var processedAddress = ProcessAddressResponce(context.Asset, r);

            //if (!this.ExchangeHas(context.Asset))
            //    return null;

            var walletAddress = new WalletAddress(this, context.Asset)
            {
                Address = processedAddress
            };

            var addresses = new WalletAddresses(walletAddress);

            return addresses;
        }

        private string ProcessAddressResponce(Asset asset, string response)
        {
            switch (asset.ShortCode)
            {
                case "BTC":
                    return response.Trim('\"');
                case "XRP":
                    var splitted = JsonConvert.DeserializeObject<BitStampSchema.AccountAddressResponse>(response).address.Split(new[] { "?dt" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length < 1)
                        throw new ApiResponseException("XRP address has incorrect format", this);
                    return splitted[0];
                case "ETH":
                case "LTC":
                    var addrr = JsonConvert.DeserializeObject<BitStampSchema.AccountAddressResponse>(response);
                    return addrr.address;
                default:
                    throw new NullReferenceException("No deposit address for specified currency");
            }
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();

            var r = await api.GetOrderBookAsync(pairCode).ConfigureAwait(false);
            var orderBook = new OrderBook(Network, context.Pair);

            var maxCount = 1000; // Used to limit number of records when no limit is set (because it's too big).

            var asks = context.MaxRecordsCount == Int32.MaxValue ? r.asks.Take(maxCount) : r.asks.Take(context.MaxRecordsCount);
            var bids = context.MaxRecordsCount == Int32.MaxValue ? r.bids.Take(maxCount) : r.bids.Take(context.MaxRecordsCount);

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.AddBid(i.Price, i.Amount);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.AddAsk(i.Price, i.Amount);

            return orderBook;
        }

        private (decimal Price, decimal Amount) GetBidAskData(decimal[] data)
        {
            decimal price = data[0];
            decimal amount = data[1];

            return (price, amount);
        }

        private string GetCurrencyPath(Asset asset)
        {
            switch (asset.ShortCode)
            {
                case "LTC":
                    return BitStampApiVersion + "/ltc_address";
                case "ETH":
                    return BitStampApiVersion + "/eth_address";
                case "BTC":
                    return "bitcoin_deposit_address";
                case "XRP":
                    return "ripple_address";
                default:
                    throw new NullReferenceException("No deposit address for specified currency");
            }
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync(context.Pair.ToTicker(this).ToLower()).ConfigureAwait(false);

            return new PublicVolumeResponse(Network, context.Pair, r.volume);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}
