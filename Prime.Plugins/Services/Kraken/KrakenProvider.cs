using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Plugins.Services.Kraken.Converters;
using Prime.Utility;

namespace Prime.Plugins.Services.Kraken
{
    // https://www.kraken.com/help/api#public-market-data
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class KrakenProvider : IBalanceProvider, IOhlcProvider, IOrderBookProvider, IPublicPricingProvider, IAssetPairsProvider, IDepositProvider
    {
        // TODO: AY implement multi-statistics.

        private const String KrakenApiUrl = "https://api.kraken.com/0";

        private RestApiClientProvider<IKrakenApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Kraken");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public bool IsDirect => true;
        public char? CommonPairSeparator { get; }

        // 'Ledger/trade history calls increase the counter by 2. ... Tier 2 users have a maximum of 15 and their count gets reduced by 1 every 3 seconds.'
        // Worst case scenario is considered here.
        // https://www.kraken.com/help/api
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(10, 1);
        public IRateLimiter RateLimiter => Limiter;

        public bool CanGenerateDepositAddress => true;
        public bool CanPeekDepositAddress => true;

        private static readonly ObjectId IdHash = "prime:kraken".GetObjectIdHashCode();
        public ObjectId Id => IdHash;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public KrakenProvider()
        {
            ApiProvider = new RestApiClientProvider<IKrakenApi>(KrakenApiUrl, this, k => new KrakenAuthenticator(k).GetRequestModifier)
            {
                JsonSerializerSettings = CreateJsonSerializerSettings()
            };
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetServerTimeAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            return r.result.unixtime > 0;
        }

        private JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                Converters = { new OhlcJsonConverter() }
            };
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body).ConfigureAwait(false);

            CheckResponseErrors(r);

            return r != null;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Bulk = new PricingBulkFeatures() { CanStatistics = true, CanVolume = true },
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairsCsv = string.Join(",", context.Pairs.Select(x => x.ToTicker(this, "")));

            var r = await api.GetTickerInformationAsync(pairsCsv).ConfigureAwait(false);

            CheckResponseErrors(r);

            var prices = new MarketPrices();
            foreach (var pair in context.Pairs)
            {
                var rTicker = r.result.Where(x => ComparePairs(pair, x.Key)).ToArray();

                if (!rTicker.Any())
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                var ticker = rTicker.First().Value;

                prices.Add(new MarketPrice(Network, pair, ticker.c[0])
                {
                    PriceStatistics = new PriceStatistics(Network, pair.Asset2, ticker.a[0], ticker.b[0], ticker.l[1], ticker.h[1]),
                    Volume = new NetworkPairVolume(Network, pair, ticker.v[1])
                });
            }

            return prices;
        }

        private bool ComparePairs(AssetPair pair, string krakenPairCode)
        {
            var result = false;
            if (krakenPairCode.Length == 6)
            {
                result = pair.Equals(krakenPairCode.ToAssetPair(this, 3));
            }
            else if (krakenPairCode.Length == 7)
            {
                result = pair.Equals(krakenPairCode.ToAssetPair(this, 3)) ||
                         pair.Equals(krakenPairCode.ToAssetPair(this, 4));
            }
            else
            {
                var pattern = @"^(([X](?<asset10>\w{3}))|((?<asset11>.\w{3})))[XZ](?<asset20>\w{3})$";
                var matches = Regex.Match(krakenPairCode, pattern);

                if (!matches.Success || !matches.Groups["asset20"].Success ||
                    (!matches.Groups["asset10"].Success && !matches.Groups["asset11"].Success))
                    return false;

                var krakenPair = new AssetPair(
                    matches.Groups["asset10"].Success
                        ? matches.Groups["asset10"].Value
                        : matches.Groups["asset11"].Value,
                    matches.Groups["asset20"].Value,
                    this
                );

                result = pair.Equals(krakenPair);
            }

            return result;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            CheckResponseErrors(r);

            var assetPairs = new AssetPairs();

            foreach (var assetPair in r.result.Where(x => !x.Key.ToLower().EndsWith(".d")).OrderBy(x => x.Value.altname.Length))
            {
                var pair = ParseAssetPair(assetPair);

                if (pair == null)
                    continue;

                assetPairs.Add(pair);
            }

            return assetPairs;
        }

        private AssetPair ParseAssetPair(KeyValuePair<string, KrakenSchema.AssetPairResponse> rPair)
        {
            var asset1 = ParseAsset(rPair.Value.base_c);
            var asset2 = ParseAsset(rPair.Value.quote);

            return new AssetPair(asset1.ToAsset(this), asset2.ToAsset(this));
        }

        private string ParseAsset(string rAsset)
        {
            var modifiers = new char[] { 'Z', 'X' };

            if (modifiers.Contains(rAsset[0]))
                return rAsset.Length <= 3 ? rAsset : rAsset.Remove(0, 1);

            return rAsset;
        }

        private Dictionary<string, object> CreateKrakenBody()
        {
            var body = new Dictionary<string, object>();
            var nonce = BaseAuthenticator.GetLongNonce();

            body.Add("nonce", nonce);

            return body;
        }

        private void CheckResponseErrors(KrakenSchema.ErrorResponse response)
        {
            if (response.error.Length > 0)
            {
                throw new ApiResponseException(response.error[0], this);
            }
        }

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
                var money = new Money(pair.Value, asset);

                results.Add(new BalanceResult(asset)
                {
                    Available = money,
                    Balance = money,
                    Reserved = 0
                });
            }

            return results;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return KrakenCodeConverterBase.I;
        }

        public async Task<string> GetFundingMethodAsync(NetworkProviderPrivateContext context, Asset asset)
        {
            var api = ApiProvider.GetApi(context);

            var body = CreateKrakenBody();
            body.Add("asset", asset.ToRemoteCode(this));

            try
            {
                var r = await api.GetDepositMethodsAsync(body).ConfigureAwait(false);

                CheckResponseErrors(r);

                return r?.result?.FirstOrDefault()?.method;
            }
            catch (ApiResponseException e)
            {
                if (e.Message.ToLower().Contains("internal error"))
                {
                    throw new ApiResponseException(
                        "Kraken internal error. Possible reason is unverified account (Tier 1 is required).");
                }
            }

            return null;
        }

        private async Task<WalletAddresses> GetAddressesLocalAsync(IKrakenApi api, string fundingMethod, Asset asset, bool generateNew = false)
        {
            var body = CreateKrakenBody();

            // BUG: do we need "aclass"?
            //body.Add("aclass", asset.ToRemoteCode(this));
            body.Add("asset", asset.ToRemoteCode(this));
            body.Add("method", fundingMethod);
            body.Add("new", generateNew);

            var r = await api.GetDepositAddressesAsync(body).ConfigureAwait(false);
            CheckResponseErrors(r);

            var walletAddresses = new WalletAddresses();

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

        public Task<TransferSuspensions> GetTransferSuspensionsAsync(NetworkProviderContext context)
        {
            return Task.FromResult<TransferSuspensions>(null);
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

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);

            var krakenTimeInterval = ConvertToKrakenInterval(context.Market);

            // BUG: "since" is not implemented. Need to be checked.
            var r = await api.GetOhlcDataAsync(context.Pair.ToTicker(this, ""), krakenTimeInterval).ConfigureAwait(false);

            CheckResponseErrors(r);

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Market, Network);

            if (r.result.pairs.Count != 0)
            {
                foreach (var ohlcResponse in r.result.pairs.FirstOrDefault().Value.OrderByDescending(x => x.time))
                {
                    var time = ((long)ohlcResponse.time).ToUtcDateTime();

                    // BUG: ohlcResponse.volume is double ~0.2..10.2, why do we cast to long?
                    ohlc.Add(new OhlcEntry(seriesId, time, this)
                    {
                        Open = (double)ohlcResponse.open,
                        Close = (double)ohlcResponse.close,
                        Low = (double)ohlcResponse.low,
                        High = (double)ohlcResponse.high,
                        VolumeTo = (long)ohlcResponse.volume, // Cast to long should be revised.
                        VolumeFrom = (long)ohlcResponse.volume,
                        WeightedAverage = (double)ohlcResponse.vwap // Should be checked.
                    });
                }
            }
            else
            {
                throw new ApiResponseException("No OHLC data received", this);
            }

            return ohlc;
        }

        private KrakenTimeInterval ConvertToKrakenInterval(TimeResolution resolution)
        {
            // BUG: Kraken does not support None, MS, S. At this moment it will throw ArgumentOutOfRangeException.
            switch (resolution)
            {
                case TimeResolution.Minute:
                    return KrakenTimeInterval.Minute1;
                case TimeResolution.Hour:
                    return KrakenTimeInterval.Hours1;
                case TimeResolution.Day:
                    return KrakenTimeInterval.Day1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private (decimal Price, decimal Volume, DateTime TimeStamp) GetBidAskData(string[] dataArray)
        {
            if (dataArray == null)
                throw new ApiResponseException("Invalid order book data response", this);

            decimal price;
            decimal volume;
            long timeStampUnix;
            DateTime timeStamp;

            if (!decimal.TryParse(dataArray[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price))
                throw new ApiResponseException("Incorrect order book data format", this);

            if (!decimal.TryParse(dataArray[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out volume))
                throw new ApiResponseException("Incorrect order book data format", this);

            if (!long.TryParse(dataArray[2], out timeStampUnix))
                throw new ApiResponseException("Incorrect order book data format", this);

            timeStamp = timeStampUnix.ToUtcDateTime();

            return (price, volume, timeStamp);
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);
            var orderBook = await GetOrderBookLocalAsync(api, context.Pair, context.MaxRecordsCount).ConfigureAwait(false);

            return orderBook;
        }

        private async Task<OrderBook> GetOrderBookLocalAsync(IKrakenApi api, AssetPair assetPair, int? maxCount)
        {
            var pair = assetPair;
            var remotePair = new AssetPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));

            var r = await api.GetOrderBookAsync(remotePair.ToTicker(this), maxCount ?? 0).ConfigureAwait(false);

            CheckResponseErrors(r);

            var data = r.result.FirstOrDefault();
            var orderBook = new OrderBook(Network, assetPair);

            var asks = maxCount.HasValue ? data.Value.asks.Take(maxCount.Value / 2).ToArray() : data.Value.asks;
            var bids = maxCount.HasValue ? data.Value.bids.Take(maxCount.Value / 2).ToArray() : data.Value.bids;

            foreach (var i in bids.Select(GetBidAskData))
                orderBook.Add(OrderType.Bid, i.Price, i.Volume);

            foreach (var i in asks.Select(GetBidAskData))
                orderBook.Add(OrderType.Ask, i.Price, i.Volume);

            return orderBook;
        }

        public async Task<PublicVolumeResponse> GetPublicVolumeAsync(PublicVolumesContext context)
        {
            var api = ApiProvider.GetApi(context);

            var remoteCode = context.Pair.ToTicker(this, "");

            var r = await api.GetTickerInformationAsync(remoteCode).ConfigureAwait(false);

            CheckResponseErrors(r);

            return new PublicVolumeResponse(Network, context.Pair, r.result.FirstOrDefault().Value.v[0]);
        }

        public VolumeFeatures VolumeFeatures { get; }
    }
}