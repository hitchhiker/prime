using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;
using Jojatekok.PoloniexAPI.MarketTools;
using LiteDB;
using Nito.AsyncEx;
using plugins.Services.CryptoCompare.Model;
using Quobject.SocketIoClientDotNet.Client;
using RestEase;
using Prime.Utility;
using vtortola.WebSockets;
using vtortola.WebSockets.Rfc6455;

namespace plugins
{
    public abstract class CryptoCompareBase : ICoinListProvider, IOhlcProvider, IPriceSocketProvider, IDisposable, IPublicPricesProvider
    {
        public static string CoinListEndpoint = "https://www.cryptocompare.com/api/data/";
        public static string CoinListEndpoint2 = "https://min-api.cryptocompare.com/data";

        private Network _network;

        public Network Network => _network ?? (_network = GetNetwork());

        public bool Disabled => false;

        public int Priority => 200;

        public virtual string AggregatorName => "CryptoCompare";

        private string _title;
        public string Title => _title ?? (_title = Name + " (" + AggregatorName + ")");

        public abstract string Name { get; }

        private Network GetNetwork()
        {
            return new Network(Name);
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

        public async Task<PriceLatest> GetLatestPrices(Asset asset, List<Asset> assets)
        {
            var api = new RestClient(CoinListEndpoint2).For<ICryptoCompareApi>();

            var apir = await api.GetPrice(asset.ToRemoteCode(this), string.Join(",", assets.Select(x => x.ToRemoteCode(this))), Name, "prime", "false", "false");
            
            var r = new PriceLatest() {UtcCreated = DateTime.UtcNow, Asset = asset};
            if (apir == null)
                return r;

            foreach (var i in apir)
            {
                var ra = i.Key.ToAsset(this);
                var a = assets.FirstOrDefault(x => x.ShortCode == ra.ShortCode);
                if (a == null)
                    continue;
                r.Prices.Add(new Money((decimal) i.Value, a));
            }
            return r;
        }

        public Task<PriceLatest> GetLatestPrice(AssetPair asset)
        {
            return GetLatestPrices(asset.Asset1, new List<Asset>() {asset.Asset2});
        }

        public List<AssetInfo> GetCoinList()
        {
            var api = RestClient.For<ICryptoCompareApi>(CoinListEndpoint);
            var apir = AsyncContext.Run(api.GetCoinListAsync);

            if (apir.IsError())
                return null;

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



        public OhclData GetOhlc(OhlcContext context)
        {
            var range = context.Range;
            var market = context.Market;
            var pair = context.Pair;

            var limit = range.GetDistanceInResolutionTicks();
            var toTs = range.UtcTo.GetSecondsSinceUnixEpoch();

            var api = RestClient.For<ICryptoCompareApi>(CoinListEndpoint2);
            var apir = AsyncContext.Run(
                delegate
                {
                    switch (market)
                    {
                        case TimeResolution.Hour:
                            return api.GetHistoricalHourly(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this), Name, "prime", "false", "true", 0, limit, toTs);
                        case TimeResolution.Day:
                            return api.GetHistoricalDay(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this), Name, "prime", "false", "true", 0, limit, toTs, "false");
                        case TimeResolution.Minute:
                            return api.GetHistoricalMinute(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this), Name, "prime", "false", "true", 0, limit, toTs);
                        default:
                            return null;
                    }
                });

            if (apir.IsError())
                return null;

            var r = new OhclData(market);
            var seriesid = OhlcResolutionDataAdapter.GetHash(pair, market, Network);
            var from = apir.TimeFrom;
            var to = apir.TimeTo;
            foreach (var i in apir.Data.Where(x=>x.time >= from && x.time<=to))
            {
                var t = ((double) i.time).UnixTimestampToDateTime();

                r.Add(new OhclEntry(seriesid, t, this)
                {
                    Open = i.open,
                    Close = i.close,
                    Low = i.low,
                    High = i.high,
                    VolumeFrom = (long)i.volumefrom,
                    VolumeTo = (long)i.volumeto
                });
            }

            if (!string.IsNullOrWhiteSpace(apir.ConversionType.conversionSymbol))
                r.ConvertedFrom = apir.ConversionType.conversionSymbol.ToAsset(this);
            
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
        */

        private Socket _priceSocket;
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

        public void Dispose()
        {
            UnSubscribePrice();
        }
    }
}