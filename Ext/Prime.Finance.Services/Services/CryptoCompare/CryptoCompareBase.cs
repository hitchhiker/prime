using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;
using RestEase;

namespace Prime.Finance.Services.Services.CryptoCompare
{
    public abstract class CryptoCompareBase : ICoinInformationProvider, IOhlcProvider, IDisposable, IPublicPricingProvider, IProxyProvider, IAssetPairsProvider
    {
        public Version Version { get; } = new Version(1, 0, 0);
        public static string EndpointLegacy = "https://www.cryptocompare.com/api/data/";
        public static string EndpointMinApi = "https://min-api.cryptocompare.com/data";

        private Network _network;

        public Network Network => _network ?? (_network = GetNetwork());

        public bool Disabled => false;

        public int Priority => 200;

        public virtual string AggregatorName => ProxyName;

        public virtual string ProxyName => "CryptoCompare";

        public bool IsDirect => false;
        public char? CommonPairSeparator { get; }

        private string _title;
        public virtual string Title => _title ?? (_title = Name + " (" + AggregatorName + ")");

        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(4000, 60);
        public IRateLimiter RateLimiter => Limiter;

        public abstract string Name { get; }

        public T GetApi<T>(bool legacyEndpoint = false) where T : class
        {
            return RestClient.For<ICryptoCompareApi>(legacyEndpoint ? EndpointLegacy : EndpointMinApi) as T;
        }

        public Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var t = new Task<bool>(() => true);
            t.Start();
            return t;
        }

        private Network GetNetwork()
        {
            return Networks.I.Get(Name);
        }

        private ObjectId GetIdHash()
        {
            return ("prime:cryptocompare:" + Name).GetObjectIdHashCode(true,true);
        }

        private ObjectId _id;
        public ObjectId Id => _id ?? (_id = GetIdHash());

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool PricesAsAssetQuotes => true;

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var p = Networks.I.Providers.FirstProviderOf<CryptoCompareProvider>();
            var r = await p.GetAssetPairsAllNetworksAsync().ConfigureAwait(false);

            var d = r.Get(Network);
            if (d == null)
                throw new Exception($"CryptoCompare does not provide asset pair information for {Network}");

            return d;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures(true, false);
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = GetApi<ICryptoCompareApi>();
            var froms = string.Join(",", context.Pairs.Select(x => x.Asset1).Distinct().Select(x => x.ShortCode));
            var tos = string.Join(",", context.Pairs.Select(x => x.Asset2).Distinct().Select(x => x.ShortCode));
            var str = await api.GetPricesAsync(froms, tos, Name, "prime", "false", "false").ConfigureAwait(false);

            if (str.Contains("market does not exist for this coin pair"))
                throw new AssetPairNotSupportedException(context.Pair, this);

            var apir = JsonConvert.DeserializeObject<CryptoCompareSchema.PriceMultiResult>(str);
            var prices = new MarketPrices();

            foreach (var i in context.Pairs)
            {
                var a1 = i.Asset1.ShortCode.ToLower();
                var k = apir.FirstOrDefault(x => x.Key.ToLower() == a1);

                if (k.Key == null)
                {
                    prices.MissedPairs.Add(i);
                    continue;
                }

                var a2 = i.Asset2.ShortCode.ToLower();
                var r = k.Value.FirstOrDefault(x => x.Key.ToLower() == a2);

                if (r.Key == null)
                {
                    prices.MissedPairs.Add(i);
                    continue;
                }

                prices.Add(new MarketPrice(Network, i, (decimal) r.Value));
            }

            return prices;
        }

        public async Task<List<AssetInfo>> GetCoinInformationAsync(NetworkProviderContext context)
        {
            var api = GetApi<ICryptoCompareApi>(true);
            var apir = await api.GetCoinListAsync().ConfigureAwait(false);

            if (apir.IsError())
                throw new ApiResponseException(apir.Response);

            var u = new UniqueList<AssetInfo>();
            foreach (var ce in apir.Data)
            {
                var v = ce.Value;
                var ai = new AssetInfo
                {
                    Remote = new Remote { Id = v.Id, ServiceId = Id },
                    Asset = v.Name.ToAsset(this),
                    FullName = v.CoinName,
                    Algorithm = v.Algorithm,
                    ProofType = v.ProofType,
                    FullyPremined = v.FullyPremined=="1",
                    TotalCoinSupply = v.TotalCoinSupply.ToDecimal(),
                    PreMinedValue = v.PreMinedValue.ToDecimal(),
                    TotalCoinsFreeFloat = v.TotalCoinsFreeFloat.ToDecimal(),
                    SortOrder = v.SortOrder.ToInt(),
                    ImageUrl = apir.BaseImageUrl + v.ImageUrl,
                    ExternalInfoUrl = apir.BaseLinkUrl + v.Url
                };

                u.Add(ai);
            }
            return u.ToList();
        }

        public async Task<OhlcDataResponse> GetOhlcAsync(OhlcContext context)
        {
            var range = context.Range;
            var resolution = context.Resolution;
            var pair = context.Pair;

            var limit = range.GetDistanceInResolutionTicks();
            var toTs = range.UtcTo.GetSecondsSinceUnixEpoch();

            var api = GetApi<ICryptoCompareApi>();
            CryptoCompareSchema.HistoricListResult apir = null;

            switch (resolution)
            {
                case TimeResolution.Hour:
                    apir = await api.GetHistoricalHourlyAsync(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this), Name, "prime", "false", "true", 0, limit, toTs).ConfigureAwait(false);
                    break;
                case TimeResolution.Day:
                    apir = await api.GetHistoricalDayAsync(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this), Name, "prime", "false", "true", 0, limit, toTs, "false").ConfigureAwait(false);
                    break;
                case TimeResolution.Minute:
                    apir = await api.GetHistoricalMinuteAsync(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this), Name, "prime", "false", "true", 0, limit, toTs).ConfigureAwait(false);
                    break;
            }

            if (apir.IsError())
                return null;

            var r = new OhlcDataResponse(resolution);
            var seriesid = OhlcUtilities.GetHash(pair, resolution, Network);
            var from = apir.TimeFrom;
            var to = apir.TimeTo;
            foreach (var i in apir.Data.Where(x=>x.time >= from && x.time<=to))
            {
                var t = ((double) i.time).UnixTimestampToDateTime();

                r.Add(new OhlcEntry(seriesid, t, this)
                {
                    Open = i.open,
                    Close = i.close,
                    Low = i.low,
                    High = i.high,
                    VolumeFrom = i.volumefrom,
                    VolumeTo = i.volumeto
                });
            }

            if (!string.IsNullOrWhiteSpace(apir.ConversionType.conversionSymbol))
                r.OhlcData.ConvertedFrom = apir.ConversionType.conversionSymbol.ToAsset(this);
            
            return r;
        }

        /*
        private WebSocket4Net.WebSocket _priceSocket;

        public void SubscribePrice(Action<string, LivePriceResponse> action)
        {
            UnSubscribePrice();

            _priceSocket = new WebSocket4Net.WebSocket("wss://streamer.cryptocompare.com");
            _priceSocket.EnableAutoSendPing = true;
            _priceSocket.Security.AllowNameMismatchCertificate = true;
            _priceSocket.Security.AllowUnstrustedCertificate = true;
            _priceSocket.Error += (sender, e) =>
            {
                var x = sender.Equals(null);
            };

            _priceSocket.MessageReceived += (sender, e) =>
            {
                action.Invoke(e.Message, MessageIn(e.Message));
            };
            _priceSocket.Opened += delegate
            {
                _priceSocket.Send("{type:'SubAdd', message: { subs: ['0~Poloniex~BTC~USD'] }}");
            };
            _priceSocket.Open();
        }

        /*
        private WebSocket _priceSocket;

        public void SubscribePrice(Action<string, LivePriceResponse> action)
        {
            UnSubscribePrice();

            _priceSocket = new WebSocket("wss://streamer.cryptocompare.com");
            _priceSocket.SslConfiguration.ServerCertificateValidationCallback = delegate
            {
                return true;
            };
            _priceSocket.OnMessage += (sender, e) => { action.Invoke(e.Data, MessageIn(e.Data)); };
            _priceSocket.OnOpen += (sender, args) => _priceSocket.Send("{type:'SubAdd', message: { subs: ['0~Poloniex~BTC~USD'] }}");
            _priceSocket.Connect();
        }
        

        public void SubscribePrice(Action<string, LivePriceResponse> action)
        {
            UnSubscribePrice();

            _priceSocket = IO.Socket("wss://streamer.cryptocompare.com");
            _priceSocket.On(Socket.EVENT_CONNECT, () =>
            {
                //L.Info("Socket Connected");
                _priceSocket.Emit("SubAdd", new object[]{"{ subs: ['0~Poloniex~BTC~USD'] }"});
            });

            _priceSocket.On(Socket.EVENT_MESSAGE, data =>
            {
                action.Invoke(data.ToString(), new LivePriceResponse());
            });

            _priceSocket.On("m", data =>
            {
                action.Invoke(data.ToString(), new LivePriceResponse());
            });

            _priceSocket.On(Socket.EVENT_DISCONNECT, () =>
            {
                //L.Info("Socket Disconnected");
            });
        }
        
        public void UnSubscribePrice()
        {
            if (_priceSocket != null)
            {
                _priceSocket.Close();
                _priceSocket = null;
            }
        }
        */
        public void Dispose()
        {
            //UnSubscribePrice();
        }
    }
}