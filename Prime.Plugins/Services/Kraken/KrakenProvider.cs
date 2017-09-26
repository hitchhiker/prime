using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prime.Core;
using Jojatekok.PoloniexAPI;
using KrakenApi;
using LiteDB;
using Newtonsoft.Json;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.Kraken;
using Prime.Utility;
using RestEase;
using AssetPair = Prime.Core.AssetPair;

namespace plugins
{
    public class KrakenProvider : IExchangeProvider, IWalletService, IOhlcProvider, IApiProvider
    {
        private const String KrakenApiUrl = "https://api.kraken.com/0";

        public Network Network { get; } = new Network("Kraken");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        [Obsolete]
        public T GetApi<T>(ApiKey key = null) where T : class
        {
            if (key==null)
                return new KrakenApi.Kraken() as T;

            return new KrakenApi.Kraken(key.Key, key.Secret) as T;
        }

        private JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                Converters = { new OhlcJsonConverter() }
            };
        }

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return new RestClient(KrakenApiUrl)
            {
                JsonSerializerSettings = CreateJsonSerializerSettings()
            }.For<IKrakenApi>() as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);

            return new RestClient(KrakenApiUrl, new KrakenAuthenticator(key).GetRequestModifier)
            {
                JsonSerializerSettings = CreateJsonSerializerSettings()
            }.For<IKrakenApi>() as T;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IKrakenApi>(context);
            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body);

            CheckResponseErrors(r);

            return r != null;
        }

        private static readonly ObjectId IdHash = "prime:kraken".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var pair = new AssetPair(context.Pair.Asset1.ToRemoteCode(this), context.Pair.Asset2.ToString());
            var remoteCode = pair.TickerKraken();

            var r = await api.GetTicketInformationAsync(remoteCode);

            CheckResponseErrors(r);

            // TODO: Check, price is taken from "last trade closed array(<price>, <lot volume>)".
            var money = new Money(r.result.FirstOrDefault().Value.c[0], context.Pair.Asset2);
            var price = new LatestPrice()
            {
                Price = money,
                BaseAsset = context.Pair.Asset1,
                UtcCreated = DateTime.Now
            };

            return price;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var r = await api.GetAssetPairsAsync();

            CheckResponseErrors(r);

            var assetPairs = new AssetPairs();

            foreach (var assetPair in r.result)
            {
                var ticker = assetPair.Key;
                var first = assetPair.Value.base_c;
                var second = ticker.Replace(first, "");

                assetPairs.Add(new AssetPair(first, second, this));   
            }

            return assetPairs;
        }

        public bool CanMultiDepositAddress { get; } = false;
        public bool CanGenerateDepositAddress { get; } = true;

        public Task<WalletAddresses> FetchAllDepositAddressesAsync(WalletAddressContext context)
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<string, object> CreateKrakenBody()
        {
            var body = new Dictionary<string, object>();
            var nonce = BaseAuthenticator.GetNonce();

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
            var api = GetApi<IKrakenApi>(context);

            var body = CreateKrakenBody();

            var r = await api.GetBalancesAsync(body);

            CheckResponseErrors(r);

            var results = new BalanceResults(this);

            foreach (var pair in r.result)
            {
                results.AddAvailable(pair.Key.ToAsset(this), pair.Value);
            }

            return results;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return KrakenCodeConverterBase.I;
        }

        public async Task<string> GetFundingMethod(NetworkProviderPrivateContext context, Asset asset)
        {
            var api = GetApi<IKrakenApi>(context);

            var body = CreateKrakenBody();
            body.Add("asset", asset.ToRemoteCode(this));

            try
            {
                var r = await api.GetDepositMethodsAsync(body);

                CheckResponseErrors(r);

                if (r == null || r.result.Count == 0)
                    return null;

                return r.result.FirstOrDefault().Value.method;
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

        public async Task<WalletAddresses> GetDepositAddressesAsync(WalletAddressAssetContext context)
        {
            // TODO: re-implement.

            var api = GetApi<IKrakenApi>(context);

            var fundingMethod = await GetFundingMethod(context, context.Asset);

            if(fundingMethod == null)
                throw new NullReferenceException("No funding method is found");

            var body = CreateKrakenBody();

            // BUG: do we need "aclass"?
            // body.Add("aclass", context.Asset.ToRemoteCode(this));
            body.Add("asset", context.Asset.ToRemoteCode(this));
            body.Add("method", fundingMethod);
            body.Add("new", false);

            // TODO: waiting for verification.

            var r = await api.GetDepositAddresses(body);

            CheckResponseErrors(r);

            var walletAddresses = new WalletAddresses();

            foreach (var addr in r.result)
            {
                var walletAddress = new WalletAddress(this, context.Asset)
                {
                    Address = addr.Value.address
                };

                if (addr.Value.expiretm != 0)
                {
                    var time = addr.Value.expiretm.ToUtcDateTime();
                    walletAddress.ExpiresUtc = time;
                }

                walletAddresses.Add(new WalletAddress(this, context.Asset) { Address = addr.Value.address });
            }

            return walletAddresses;
        }

        public async Task<OhclData> GetOhlcAsync(OhlcContext context)
        {
            var api = GetApi<IKrakenApi>(context);

            var pair = new AssetPair(context.Pair.Asset1.ToRemoteCode(this), context.Pair.Asset2.ToString());

            var krakenTimeInterval = ConvertToKrakenInterval(context.Market);

            // BUG: "since" is not implemented. Need to be checked.
            var r = await api.GetOhlcDataAsync(pair.TickerKraken(), krakenTimeInterval);

            CheckResponseErrors(r);

            var ohlc = new OhclData(context.Market);
            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            if (r.result.pairs.Count != 0)
            {
                foreach (var ohlcResponse in r.result.pairs.FirstOrDefault().Value)
                {
                    var time = ((long)ohlcResponse.time).ToUtcDateTime();

                    // BUG: ohlcResponse.volume is double ~0.2..10.2, why do we cast to long?
                    ohlc.Add(new OhclEntry(seriesId, time, this)
                    {
                        Open = (double) ohlcResponse.open,
                        Close = (double) ohlcResponse.close,
                        Low = (double) ohlcResponse.low,
                        High = (double) ohlcResponse.high,
                        VolumeTo = (long) ohlcResponse.volume, // Cast to long should be revised.
                        VolumeFrom = (long) ohlcResponse.volume,
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
    }
}